#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <netdb.h>

#define BUFSIZE  1000

using namespace std;
using byte = char;


class Protocol
{
public:
    //Call funktions
    bool serverOpen(string IP, int PORT);
    void serverClose();

    bool tjekMessage();

    //Only for use in protocol
    string reciveMsg();
    bool sendMsg(string  msg);
    
private:
    char buffer[BUFSIZE] = {0};
    string request;
    int client = socket(AF_INET, SOCK_STREAM, 0);
};
