#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdlib.h>
#include <cstring>
#include <unistd.h>


using namespace std;

//Usings
using byte = char;

//Definations
#define BUFSIZE  1000
//#define PORT    4952
//#define IP    "84.238.30.243"

//Own libs
#include "protocol.h"



//Variables
struct sockaddr_in serv_addr;
char buffer[BUFSIZE] = {0};
string request;
int client = socket(AF_INET, SOCK_STREAM, 0);


bool Protocol::serverOpen(string IP, int PORT)
{
	int clientWaiter = socket(AF_INET, SOCK_STREAM, 0);
	// Indstiller server-sock_addr_in
    memset((char *) &serv_addr, 0, sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_port = htons(PORT);

        // Saetter IP-adresse
    if (inet_aton(IP.c_str() , &serv_addr.sin_addr) == 0)
            cout << "ERROR on inet_aton()" << endl;

    // Opretter client socket
	if (connect(clientWaiter , (struct sockaddr *)&serv_addr , sizeof(serv_addr)) < 0)
	{
        cout << "Could not create socket" << endl;
        return false;
    }
    else
    {
    	cout << "Socket created" << endl;
    	client = clientWaiter;
    	return true;
    }
}


void Protocol::serverClose()
{
	close(client);
}

bool Protocol::tjekMessage()
{
	//Bruges til at tjekke for beskeder
	fd_set sockset;
    FD_ZERO(&sockset);
    FD_SET(client, &sockset);
	int result = select(client + 1, &sockset, NULL, NULL, NULL);
	if( result == 1)
	{
		return true;
	}
	else 
	{
		return false;
	}
}


string  Protocol::reciveMsg()
{
	byte bufRecive[BUFSIZE] = {0};

	if(recv(client,bufRecive,sizeof(bufRecive), 0) < 0)
	{
        cout << "ERROR on recive()" << endl;
    }

    string buf = string(bufRecive, strlen(bufRecive));
	return buf ;
}



bool Protocol::sendMsg(string  msg)
{
	//send(client, msg, strlen(msg),0)

	if(sendto(client, msg.c_str(), msg.length(), 0, 
	(struct sockaddr *) &serv_addr, (socklen_t)sizeof(serv_addr)) < 0)
	{
        cout << "ERROR on send(byte*  msg)" << endl;
        return false;
    }
	return true;
}

