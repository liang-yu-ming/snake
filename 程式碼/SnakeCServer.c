#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>
#include <time.h>
#include <pthread.h>
#include <unistd.h>
#include <errno.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>
#include <arpa/inet.h>
#include <sys/wait.h>
#include <signal.h>
#define MAX_PLAYER_NUMBER 5
#define QUEUE_SIZE MAX_PLAYER_NUMBER+1
#define PORT "8888"

typedef struct Tuple {
    int x;              //從左上角往下增加
    int y;              //從左上角往右增加
} Tuple;

typedef struct Player {
    char Ip[20];        //玩家的IP
    char Name[10];      //玩家的名稱
    Tuple Snake[50];    //蛇從頭到尾每個點的位置
                        //初始化時需給入(3,3), (4,3), (5,3)
    int SnakeLength;    //蛇的長度
    char ColorName[10]; //蛇的顏色
    int Direction;      //玩家當前行進方向，此項不序列化
} Player;

typedef struct ServerPacket {
    char *IP;        //IP字串
    bool IsDead;        //Server告訴玩家是否死亡
    //Player Players[MAX_PLAYER_NUMBER];  //所有玩家的資訊
    //int Pointx;         //點點的X座標
    //int Pointy;         //點點的Y座標
} ServerPacket;

typedef struct ClientPacket {
    char IP[20];        //IP字串
    char Name[10];      //玩家傳送名稱
    int Presskey;       //玩家的操作資訊
} ClientPacket;

void sigchld_handler(int s) {
    while (waitpid(-1, NULL, WNOHANG) > 0);
}

void *get_in_addr(struct sockaddr *sa) {
    if (sa->sa_family == AF_INET) {
        return &(((struct sockaddr_in *)sa)->sin_addr);
    }
    return &(((struct sockaddr_in6 *)sa)->sin6_addr);
}
void *Send();
void *ManageGame();                     //處理玩家移動、死亡和吃點點，生成新點點
void *ManagePlayer();                   //移除斷線玩家，和新增玩家
int FindPlayer(int);                    //依據socket_fd回傳玩家索引值
char *Serialize(ServerPacket, char *);  //將ServerPacket序列化
ClientPacket Deserialize(char *);       //將ClientPacket反序列化
void Move(int, int);                    //蛇的移動
void Eat(int);                          //蛇吃到食物
int Cal(int);                           //計算蛇的移動方向
Tuple NewPoint(int, int);               //計算一個符合限制的新點

bool running = true;
char localIP[20];                               //本機ip
Player players[MAX_PLAYER_NUMBER];              //玩家資訊
bool isPlayers[MAX_PLAYER_NUMBER];              //紀錄players是否存在玩家
int playerActions[MAX_PLAYER_NUMBER];           //紀錄玩家的操作資訊
Tuple point = {15, 15};                         //點點位置
char colorNames[6][10] = {"Red", "Blue", "Green", "Yellow", "Purple", ""};      //存入可用的顏色
int colorNamesBegin = 0, colorNamesEnd = 5;
int waitingQueue[QUEUE_SIZE];                   //存入待新增至遊戲的玩家
int waitingQueueBegin = 0, waitingQueueEnd = 0;
int removeQueue[QUEUE_SIZE];                    //存入因斷線而待移除的玩家
int removeQueueBegin = 0, removeQueueEnd = 0;
bool isDeadPlayers[MAX_PLAYER_NUMBER];          //紀錄死亡的玩家
int playersSocket[MAX_PLAYER_NUMBER];           //紀錄玩家的fd編號
//---------------------------------------------------------
int listener; //file descriptor 紀錄類似 socket 編號的東東
fd_set master; // master file descriptor 清單
int fdmax; // 最大的 file descriptor 數目
int nbytes; // 從 client 接收到的資料大小
char lenbuf[4], buf[256]; // 儲存 client 資料的緩衝區
char sendMsg[11000]; // 傳送給client的資料內容
//---------------------------------------------------------
int main() {
    srand(time(NULL));
    pthread_t managePlayerThread, manageGameThread, sendThread;
    pthread_create(&manageGameThread, NULL, ManageGame, NULL);      //檢查點點是否被吃掉
    pthread_create(&managePlayerThread, NULL, ManagePlayer, NULL);  //管理玩家進出遊戲
    //-----------------------------------------------------
    int newfd;
    struct addrinfo hints, *servinfo, *p;
    struct sockaddr_storage remoteaddr;
    struct sigaction sa;
    socklen_t addrlen;
    int yes = 1, rv;
    fd_set read_fds; // 給 select() 用的暫時 file descriptor 清單
    struct timeval timeout; // select的time out如果時間超過了，
                        //而 select() 還沒有找到任何就緒的 file descriptor 時，
                        //它就會返回，讓你可以繼續做其它事情。
    //server 初始化開始
    memset(&hints, 0, sizeof hints);// 確保 struct 為空
    hints.ai_family = AF_UNSPEC;// 不用管是 IPv4 或 IPv6
    hints.ai_socktype = SOCK_STREAM; // TCP stream sockets
    hints.ai_flags = AI_PASSIVE;// 幫我填好我的 IP 

    if ((rv = getaddrinfo(NULL, PORT, &hints, &servinfo)) != 0) {
        fprintf(stderr, "getaddrinfo: %s\n", gai_strerror(rv));
        return 1;
    } //將 ip 和 port 放進一個地址 &servinfo

    for (p = servinfo; p != NULL; p = p->ai_next) {
        if ((listener = socket(p->ai_family, p->ai_socktype, p->ai_protocol)) == -1) {
            perror("server: socket");
            continue;
        } //建立一個 socket 用來當作 listener
        if (setsockopt(listener, SOL_SOCKET, SO_REUSEADDR, &yes, sizeof(int)) == -1) {
            perror("setsockopt");
            exit(1);
        } //重新使用先前使用的 port
        if (bind(listener, p->ai_addr, p->ai_addrlen) == -1) {
            close(listener);
            perror("server: bind");
            continue;
        } //將 socket bind 到本機 ip 上
        break;
    } //利用 &servinfo 內的資訊建立 socket 並 bind
    if (p == NULL) {
        fprintf(stderr, "server: failed to bind\n");
        return 2;
    }

    freeaddrinfo(servinfo); //使用完 &servinfo 後釋放他

    if (listen(listener, MAX_PLAYER_NUMBER) == -1) {
        perror("listen");
        exit(1);
    } //開始監聽 listener

    FD_ZERO(&master); // 確保 master set 是空的
    FD_SET(listener, &master); // 將 listener 新增到 master set
    fdmax = listener; // 預設最大的 file descriptor 為listener

    timeout.tv_sec = 3;
    timeout.tv_usec = 0;

    sa.sa_handler = sigchld_handler; // 收拾全部死掉的 processes
    sigemptyset(&sa.sa_mask);
    sa.sa_flags = SA_RESTART;

    if (sigaction(SIGCHLD, &sa, NULL) == -1) {
        perror("sigaction");
        exit(1);
    }

    printf("server: waiting for connections...\n");
    pthread_create(&sendThread, NULL, Send, NULL);          //管理玩家進出遊戲
    //server 初始化結束
    while (1) {
        read_fds = master; // 複製 master
        int check = select(fdmax + 1, &read_fds, NULL, NULL, NULL);
        if (check < 0) {
            perror("select");
            exit(4);
        }
        else if (check == 0) {
            //printf("select() timed out.  End program.\n");
            //break;
        }

        for (int i = 0; i <= fdmax; i++) { // 在現存的連線中尋找需要讀取的資料
            if (FD_ISSET(i, &read_fds)) { // 我們找到一個！！
                if (i == listener) { // 確認 listener 是否接收到新連線
                    printf("Listening socket is readable\n");

                    addrlen = sizeof remoteaddr;
                    newfd = accept(listener, (struct sockaddr *)&remoteaddr, &addrlen); // 接收新連線
                    if (newfd == -1) {
                        perror("accept");
                    }
                    else {
                        FD_SET(newfd, &master); // 新增到 master set
                        if (newfd > fdmax) { // 持續追蹤最大的 fd
                            fdmax = newfd;
                        }
                        char s[INET6_ADDRSTRLEN];
                        inet_ntop(remoteaddr.ss_family, get_in_addr((struct sockaddr *)&remoteaddr), s, sizeof s);
                        printf("selectserver: new connection from %s on socket %d\n", s, newfd);

                        waitingQueue[waitingQueueEnd++] = newfd;
                        waitingQueueEnd %= QUEUE_SIZE;
                    }
                }
                else {
                    printf("Descriptor %d is readable\n", i);
                    if ((nbytes = recv(i, lenbuf, sizeof lenbuf, 0)) <= 0) { // 處理來自 client 的資料
                        if (nbytes == 0) {
                            printf("selectserver: socket %d died\n", i);
                        }
                        else {
                            perror("recv");
                        }
                        close(i);
                        FD_CLR(i, &master); // 從 master set 中移除
                        removeQueue[removeQueueEnd++] = FindPlayer(i);
                        removeQueueEnd %= QUEUE_SIZE;
                    }
                    else if (nbytes > 0) {
                        int len = 0;
                        for (int j = 0;j < 4;j += 1) {
                            len *= 10;
                            len += lenbuf[j] - '0';
                        }
                        printf("from socket %d receive = %s\n", i, lenbuf);
                        if ((nbytes = recv(i, buf, len, 0)) > 0) {
                            ClientPacket clientPacket = Deserialize(buf);
                            int ind = FindPlayer(i);
                            if (ind != -1) {
                                strcpy(players[ind].Ip, clientPacket.IP);
                                strcpy(players[ind].Name, clientPacket.Name);
                                playerActions[ind] = clientPacket.Presskey;
                            }
                        }
                    }
                } // END handle data from client
            } // i 有在 read_fds 中
        } // END looping through file descriptors
    } // END while(1) and you thought it would never end!
    //-----------------------------------------------------
    pthread_join(manageGameThread, NULL);
    pthread_join(managePlayerThread, NULL);
    pthread_join(sendThread, NULL);
}

void *Send() {
    while (running) {
        for (int i = 0; i <= fdmax; i++) { // 廣播給所有人！
            if (FD_ISSET(i, &master)) {
                if (i != listener) { // 不用送給 listener
                    int sendcount = -1;
                    int ind = FindPlayer(i);
                    if (ind != -1) {
                        ServerPacket serverPacket = {
                            .IP = players[ind].Ip,
                            .IsDead = isDeadPlayers[ind]
                        };
                        char buffer[11000];
                        Serialize(serverPacket, buffer);
                        printf("Send : %s\n", buffer);
                        int sendMsgLen = strlen(buffer);
                        char lenstr[5] = {'0' + (sendMsgLen / 1000), '0' + (sendMsgLen / 100) % 10, '0' + (sendMsgLen / 10) % 10, '0' + (sendMsgLen % 10), '\0'};
                        strcat(sendMsg, lenstr);
                        strcat(sendMsg, buffer);
                        sendMsgLen += 4;
                        while (sendcount < sendMsgLen) {
                            sendcount = send(i, sendMsg, sendMsgLen, 0);
                            printf("already send %d bytes\n", sendcount);
                            if (sendcount == -1) {
                                perror("send");
                                break;
                            }
                        }
                        memset(sendMsg, 0, 11000);
                    }
                }
            }
        }
        usleep(50 * 1000);
    }
    pthread_exit(NULL);
}

void *ManageGame() {
    while (running) {
        bool check = false;
        for (int i = 0; i < MAX_PLAYER_NUMBER; i += 1) {
            if (isPlayers[i]) {
                if (playerActions[i] != -1) {
                    Move(i, playerActions[i]);
                    playerActions[i] = -1;
                }
                else {
                    Move(i, players[i].Direction);
                }
            }
        }
        for (int i = 0; i < MAX_PLAYER_NUMBER; i += 1) {
            for (int j = 0; j < MAX_PLAYER_NUMBER; j += 1) {
                if (i == j || !isPlayers[i] || !isPlayers[j])
                    continue;
                for (int k = 0;k < players[i].SnakeLength;k += 1)
                    if (players[i].Snake[k].x == players[j].Snake[0].x && players[i].Snake[k].y == players[j].Snake[0].y)
                        isDeadPlayers[j] = true;
            }
        }
        for (int i = 0;i < MAX_PLAYER_NUMBER;i += 1) {
            if (isPlayers[i]) {
                if (point.x == players[i].Snake[0].x && point.y == players[i].Snake[0].y) {
                    Eat(i);
                    check = true;
                }
            }
        }
        if (check) {
            point.x = -1;
            point.y = -1;
            while (true) {
                int x = rand() % 30;
                int y = rand() % 30;
                Tuple newPoint = {x, y};
                bool isSpace = true;
                for (int i = 0;i < MAX_PLAYER_NUMBER;i += 1) {
                    if (isPlayers[i]) {
                        for (int j = 0;j < players[i].SnakeLength;j += 1) {
                            if (players[i].Snake[j].x == newPoint.x && players[i].Snake[j].y == newPoint.y) {
                                isSpace = false;
                                break;
                            }
                        }
                    }
                }
                if (isSpace) {
                    point = newPoint;
                    break;
                }
            }
        }
        usleep(200 * 1000);
    }
    pthread_exit(NULL);
}

void *ManagePlayer() {
    while (running) {
        while (removeQueueBegin != removeQueueEnd) {
            int ind = removeQueue[removeQueueBegin++];
            removeQueueBegin %= QUEUE_SIZE;
            strcpy(colorNames[colorNamesEnd++], players[ind].ColorName);
            colorNamesEnd %= QUEUE_SIZE;
            isPlayers[ind] = false;
            isDeadPlayers[ind] = false;
        }
        while (waitingQueueBegin != waitingQueueEnd) {
            Player player = {
                .Ip = "",
                .Snake = {{3, 3}, {4, 3}, {5, 3}},
                .SnakeLength = 3
            };
            strcpy(player.ColorName, colorNames[colorNamesBegin++]);
            colorNamesBegin %= QUEUE_SIZE;
            for (int i = 0;i < MAX_PLAYER_NUMBER;i += 1) {
                if (!isPlayers[i]) {
                    players[i] = player;
                    isPlayers[i] = true;
                    playersSocket[i] = waitingQueue[waitingQueueBegin++];
                    waitingQueueBegin %= QUEUE_SIZE;
                    break;
                }
            }
        }
        usleep(200 * 1000);
    }
    pthread_exit(NULL);
}

int FindPlayer(int fd) {
    for (int i = 0;i < MAX_PLAYER_NUMBER;i += 1) {
        if (isPlayers[i]) {
            if (playersSocket[i] == fd) {
                return i;
            }
        }
    }
    return -1;
}

char *Serialize(ServerPacket serverPacket, char *buffer) {
    const char *tupleFormat = "{\"Item1\":%d,\"Item2\":%d}";
    const char *playerFormat = "{\"Ip\":\"%s\",\"Name\":\"%s\",\"Snake\":[%s],\"ColorName\":\"%s\"}";
    const char *sendOutputFormat = "{\"IP\":\"%s\",\"IsDead\":%s,\"Players\":[%s],\"Pointx\":%d,\"Pointy\":%d}";
    bool isFirst = true;
    char playerString[10000];
    memset(playerString, 0, 10000);
    for (int i = 0;i < 5;i += 1) {
        if (isPlayers[i]) {
            char snakeString[1500], snakeBuffer[25], playerBuffer[2000];
            memset(snakeString, 0, 1500);
            sprintf(snakeBuffer, tupleFormat, players[i].Snake[0].x, players[i].Snake[0].y);
            strcat(snakeString, snakeBuffer);
            for (int j = 1;j < players[i].SnakeLength;j += 1) {
                sprintf(snakeBuffer, tupleFormat, players[i].Snake[j].x, players[i].Snake[j].y);
                strcat(snakeString, ",");
                strcat(snakeString, snakeBuffer);
            }
            sprintf(playerBuffer, playerFormat, players[i].Ip, players[i].Name, snakeString, players[i].ColorName);
            if (isFirst)
                isFirst = false;
            else
                strcat(playerString, ",");
            strcat(playerString, playerBuffer);
        }
    }
    sprintf(buffer,
            sendOutputFormat,
            serverPacket.IP,
            serverPacket.IsDead ? "true" : "false",
            playerString,
            point.x,
            point.y
    );
    return buffer;
}

ClientPacket Deserialize(char *JSONMsg) {
    ClientPacket clientPacket;
    const char *getInputFormat = "{\"IP\":\"%[^\"]\",\"Name\":\"%[^\"]\",\"Presskey\":%d}";
    sscanf(JSONMsg, getInputFormat,
           clientPacket.IP,
           &clientPacket.Name,
           &clientPacket.Presskey
    );
    return clientPacket;
}

void Move(int ind, int dir) {
    // 0:up 1:down 2:left 3:right
    if (dir == 0 && players[ind].Direction == 1)
        dir = players[ind].Direction;
    else if (dir == 1 && players[ind].Direction == 0)
        dir = players[ind].Direction;
    else if (dir == 2 && players[ind].Direction == 3)
        dir = players[ind].Direction;
    else if (dir == 3 && players[ind].Direction == 2)
        dir = players[ind].Direction;
    players[ind].Direction = dir;
    for (int i = players[ind].SnakeLength; i > 0; i--) {
        players[ind].Snake[i].x = players[ind].Snake[i - 1].x;
        players[ind].Snake[i].y = players[ind].Snake[i - 1].y;
    }
    switch (dir) {
        case 0:
            players[ind].Snake[0] = NewPoint(players[ind].Snake[0].x - 1, players[ind].Snake[0].y);
            break;
        case 1:
            players[ind].Snake[0] = NewPoint(players[ind].Snake[0].x + 1, players[ind].Snake[0].y);
            break;
        case 2:
            players[ind].Snake[0] = NewPoint(players[ind].Snake[0].x, players[ind].Snake[0].y - 1);
            break;
        case 3:
            players[ind].Snake[0] = NewPoint(players[ind].Snake[0].x, players[ind].Snake[0].y + 1);
            break;
    }
}

void Eat(int ind) {
    int dir = Cal(ind);
    switch (dir) {
        case 0:
            players[ind].Snake[players[ind].SnakeLength] = NewPoint(players[ind].Snake[players[ind].SnakeLength - 1].x - 1, players[ind].Snake[players[ind].SnakeLength - 1].y);
            break;
        case 1:
            players[ind].Snake[players[ind].SnakeLength] = NewPoint(players[ind].Snake[players[ind].SnakeLength - 1].x + 1, players[ind].Snake[players[ind].SnakeLength - 1].y);
            break;
        case 2:
            players[ind].Snake[players[ind].SnakeLength] = NewPoint(players[ind].Snake[players[ind].SnakeLength - 1].x, players[ind].Snake[players[ind].SnakeLength - 1].y - 1);
            break;
        case 3:
            players[ind].Snake[players[ind].SnakeLength] = NewPoint(players[ind].Snake[players[ind].SnakeLength - 1].x, players[ind].Snake[players[ind].SnakeLength - 1].y + 1);
            break;
    }
    players[ind].SnakeLength += 1;
}

int Cal(int ind) {
    int dx = players[ind].Snake[players[ind].SnakeLength - 1].x - players[ind].Snake[players[ind].SnakeLength - 2].x;
    int dy = players[ind].Snake[players[ind].SnakeLength - 1].y - players[ind].Snake[players[ind].SnakeLength - 2].y;
    if (dx == -1 && dy == 0)
        return 0;
    if (dx == 1 && dy == 0)
        return 1;
    if (dx == 0 && dy == -1)
        return 2;
    if (dx == 0 && dy == 1)
        return 3;
    return 0;
}

Tuple NewPoint(int a, int b) {
    if (a < 0)
        a = 29;
    if (a > 29)
        a = 0;
    if (b < 0)
        b = 29;
    if (b > 29)
        b = 0;
    Tuple point = {a, b};
    return point;
}