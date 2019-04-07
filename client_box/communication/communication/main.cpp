#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdlib.h>
#include <string>
#include <unistd.h>
#include <pthread.h>
#include <mutex>
#include <deque>



#include "protocol.h"

using namespace std;


void *telnet_tread(void *ptr);

mutex m;


struct connectInfo{
	int port_;
	string ip_;
    int id_;
};




string addresser[] = {"192.168.43.8" , "192.168.43.125" , "192.168.43.224"};

string stades[] = {"available", "available", "available"};
string who[] = {"H4:E62D:45:6F:B4" , "H4:E62D:45:6F:B4" , "H4:E62D:45:6F:B4"};

bool online = false;

int main(int argc, char *argv[])
{
    Protocol protocol;
    bool openConnection = false; //Fortæller at forbindelsen til serveren er oprettet
	pthread_t thread[3];
    connectInfo msg[3];
    
	for(int i = 0; i < 3 ; i++)
	{
		msg[i].port_ = 10023 + i;
		msg[i].ip_ = addresser[i];
        msg[i].id_ = i;
		pthread_create( &thread[i], NULL, telnet_tread, &msg[i]);
	}
    
    
    
    bool connectionOn = false;
    
	while(true)
	{

		cout << "opening connection to box" << endl;
		openConnection = false;

		while(openConnection == false)//Venter på der er oprettet forbindelse til server
		{
            m.lock();
			openConnection = protocol.serverOpen("192.168.43.54", 4952);
            m.unlock();
			if(openConnection == false)
			{
                connectionOn = false;
			}
			else
            {
                m.lock();
                connectionOn = true;
                online = true;
                m.unlock();
            }
        }

		while(connectionOn)
		{
            
			cout << "IM ALIVE TO SERVER" << endl;

            sleep(5);
			//m.lock();
            for(int i = 0; i < 3 ; i++)
            {
                protocol.sendMsg("1");
                sleep(1);
                //protocol.reciveMsg();
                protocol.sendMsg(who[i]+'\0');
                sleep(1);
                //protocol.reciveMsg();
                protocol.sendMsg(stades[i]+'\0');
                sleep(3);
            }
            //m.unlock();

			string menu = "0";

			if(protocol.tjekMessage() == true)
			{
				cout << "Message from server" << endl;
                m.lock();
				menu = protocol.reciveMsg();
                m.unlock();

				if(menu == "1")
				{
					cout << "Server want status" << endl;
                    
                    
				}
				else if(menu == "2")
				{
					cout << "Deaktiver alarm" << endl;
    
				}
				else
				{
					cout << "not a valide menu input" << endl;
					cout << "closing server" << endl;
					protocol.serverClose();
                    connectionOn = false;
				}
			}
			
			
		}
		online = false;

	}

		return (1);
		//////////////// TIL PROTOCOL ////////////////////
}


void *telnet_tread(void *ptr)
{
    bool openConnection = false; //Fortæller at forbindelsen til serveren er oprettet
	connectInfo * info = (connectInfo *)ptr;
	int port = info->port_;
	string IP = info->ip_;
    int id = info->id_;


    cout << "opening connection to box" << endl;
    Protocol protocolEsp;

    bool connectionOn = false;
    string msgEsp;
    string addrEsp;
    
    while(true)
    {

        cout << "opening connection to box" << endl;
        openConnection = false;

        while(openConnection == false)//Venter på der er oprettet forbindelse til server
        {
            sleep(1);
            m.lock();
            openConnection = protocolEsp.serverOpen(IP, port);
            m.unlock();
            
            if(openConnection == false)
            {
                sleep(1);
                connectionOn = false;
            }
            else
            {
                connectionOn = true;
            }
        }

        while(connectionOn)
        {

            cout << "IM ALIVE ESP:" << IP << endl;

            sleep(5);
            m.lock();
            protocolEsp.sendMsg("m");
            m.unlock();
            

            if(protocolEsp.tjekMessage() == true)
            {
                m.lock();
                msgEsp = protocolEsp.reciveMsg();
                m.unlock();
                cout << "Message from ESP:"<< IP << " , " << msgEsp << endl;
            }
            
            m.lock();
            protocolEsp.sendMsg("p");
            m.unlock();
            

            if(protocolEsp.tjekMessage() == true)
            {
                m.lock();
                addrEsp = protocolEsp.reciveMsg();
                m.unlock();
                cout << "Message from ESP:"<< IP << " , " << addrEsp << endl;
            }
            
            
            m.lock();
            stades[id] = msgEsp;
            who[id] = addrEsp;
            m.unlock();
        }

    }
	
}
