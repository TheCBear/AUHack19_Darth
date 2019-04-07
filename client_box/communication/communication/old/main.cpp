#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdlib.h>
#include <string>
#include <unistd.h>
#include <pthread.h>
#include <mutex>
#include <deque>
#include <boost/bind.hpp>
#include <boost/asio.hpp>
#include <boost/thread.hpp>


#ifdef POSIX
#include <termios.h>
#endif

#include "protocol.h"


using boost::asio::ip::tcp;
using namespace std;


void *telnet_tread(void *ptr);


string buffer; //Den modtaget string fra server
bool msgSucces = false; //Fortæller om beskeden blev sendt
bool openConnection = false; //Fortæller at forbindelsen til serveren er oprettet

string sendString; //Benyt denne variable til at ligge jeres besked i til afsending til server
int sendInt; //Benyt denne string til at ligge jeres besked i til afsending til server
mutex lockRFID;


string alarmState = "0";

struct connectInfo{
	int port_;
	string ip_;
};


int main(int argc, char *argv[])
{
	pthread_t thread[1];
	for(int i = 0; i < 1 ; i++)
	{
		connectInfo msg;
		msg.port_ = 23;
		msg.ip_ = "192.168.43.8";
		pthread_create( &thread[i], NULL, telnet_tread, &msg);
	}

	while(true)
	{

		cout << "opening connection to box" << endl;
		openConnection = false;

		while(openConnection == false)//Venter på der er oprettet forbindelse til server
		{
			sleep(1);
			openConnection = serverOpen();
			if(openConnection == false)
			{
				//sendSeriel("7");
				sleep(1);
				//alarmState = "1";
			}
		}

		while(true)
		{
			cout << "IM ALIVE" << endl;



			string menu = "0";

			if(tjekMessage() == true)
			{
				cout << "Message from server" << endl;
				menu = reciveMsg();

				if(menu == "1")
				{
					cout << "Server want alarm status" << endl;

					if(alarmState == "1")
						sendMsg("1");
					else
						sendMsg("0");

				}
				else if(menu == "2")
				{
					cout << "Deaktiver alarm" << endl;

				}
				else if(menu == "3") //ping
				{
					cout << "Server want to hire, is the box alive" << endl;
		//			sendSeriel("3");
			//		string state = recevieSeriel();
				//	if(state == "1")
						sendMsg("1");
					//else
						//sendMsg("0");

				}
				else if(menu == "4")
				{
					cout << "Server want to open box" << endl;
				}
				else if(menu == "5") //close connection
				{
					cout << "Server want to close connection" << endl;
					serverClose();
				}
				else if(menu == "6") //close connection
				{
					string sensi = reciveMsg();
					cout << "Server want to set sensitiviti" << endl;
				}
				else
				{
					cout << "not a valide menu input" << endl;
					cout << "closing server" << endl;
					//serverClose();
					break;
				}
			}
		}

	}

		return (1);
		//////////////// TIL PROTOCOL ////////////////////
}



class telnet_client
{
public:
	enum { max_read_length = 512 };

	telnet_client(boost::asio::io_service& io_service, tcp::resolver::iterator endpoint_iterator)
		: io_service_(io_service), socket_(io_service)
	{
		connect_start(endpoint_iterator);
	}

	void write(const char msg) // pass the write data to the do_write function via the io service in the other thread
	{
		io_service_.post(boost::bind(&telnet_client::do_write, this, msg));
	}

	void close() // call the do_close function via the io service in the other thread
	{
		io_service_.post(boost::bind(&telnet_client::do_close, this));
	}

private:

	void connect_start(tcp::resolver::iterator endpoint_iterator)
	{ // asynchronously connect a socket to the specified remote endpoint and call connect_complete when it completes or fails
		tcp::endpoint endpoint = *endpoint_iterator;
		socket_.async_connect(endpoint,
			boost::bind(&telnet_client::connect_complete,
				this,
				boost::asio::placeholders::error,
				++endpoint_iterator));
	}

	void connect_complete(const boost::system::error_code& error, tcp::resolver::iterator endpoint_iterator)
	{ // the connection to the server has now completed or failed and returned an error
		if (!error) // success, so start waiting for read data
			read_start();
		else if (endpoint_iterator != tcp::resolver::iterator())
		{ // failed, so wait for another connection event
			socket_.close();
			connect_start(endpoint_iterator);
		}
	}

	void read_start(void)
	{ // Start an asynchronous read and call read_complete when it completes or fails
		socket_.async_read_some(boost::asio::buffer(read_msg_, max_read_length),
			boost::bind(&telnet_client::read_complete,
				this,
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred));
	}

	void read_complete(const boost::system::error_code& error, size_t bytes_transferred)
	{ // the asynchronous read operation has now completed or failed and returned an error
		if (!error)
		{ // read completed, so process the data
			cout.write(read_msg_, bytes_transferred); // echo to standard output
			//cout << "\n";
			read_start(); // start waiting for another asynchronous read again
		}
		else
			do_close();
	}

	void do_write(const char msg)
	{ // callback to handle write call from outside this class
		bool write_in_progress = !write_msgs_.empty(); // is there anything currently being written?
		write_msgs_.push_back(msg); // store in write buffer
		if (!write_in_progress) // if nothing is currently being written, then start
			write_start();
	}

	void write_start(void)
	{ // Start an asynchronous write and call write_complete when it completes or fails
		boost::asio::async_write(socket_,
			boost::asio::buffer(&write_msgs_.front(), 1),
			boost::bind(&telnet_client::write_complete,
				this,
				boost::asio::placeholders::error));
	}

	void write_complete(const boost::system::error_code& error)
	{ // the asynchronous read operation has now completed or failed and returned an error
		if (!error)
		{ // write completed, so send next write data
			write_msgs_.pop_front(); // remove the completed data
			if (!write_msgs_.empty()) // if there is anthing left to be written
				write_start(); // then start sending the next item in the buffer
		}
		else
			do_close();
	}

	void do_close()
	{
		socket_.close();
	}

private:
	boost::asio::io_service& io_service_; // the main IO service that runs this connection
	tcp::socket socket_; // the socket this instance is connected to
	char read_msg_[max_read_length]; // data read from the socket
	deque<char> write_msgs_; // buffered write data
};



void *telnet_tread(void *ptr)
{
	connectInfo * info = (connectInfo *)ptr;
	int argc = info->port_;
	const char* argv = info->ip_.c_str();
	const char* port = "23";

	while (true)
	{
		#ifdef POSIX
			termios stored_settings;
			tcgetattr(0, &stored_settings);
			termios new_settings = stored_settings;
			new_settings.c_lflag &= (~ICANON);
			new_settings.c_lflag &= (~ISIG); // don't automatically handle control-C
			tcsetattr(0, TCSANOW, &new_settings);
		#endif
			try
			{
				if (argc != 3)
				{
					cerr << "Usage: telnet <host> <port>\n";
				}
				boost::asio::io_service io_service;
				// resolve the host name and port number to an iterator that can be used to connect to the server
				tcp::resolver resolver(io_service);
				tcp::resolver::query query(argv, port);
				tcp::resolver::iterator iterator = resolver.resolve(query);
				// define an instance of the main class of this program
				telnet_client c(io_service, iterator);
				// run the IO service as a separate thread, so the main thread can block on standard input
				boost::thread t(boost::bind(&boost::asio::io_service::run, &io_service));
				while (1)
				{
					char ch;
					cin.get(ch); // blocking wait for standard input
					if (ch == 3) // ctrl-C to end program
						break;
					c.write(ch);
				}
				c.close(); // close the telnet client connection
				t.join(); // wait for the IO service thread to close
			}
			catch (exception& e)
			{
				cerr << "Exception: " << e.what() << "\n";
			}
		#ifdef POSIX // restore default buffering of standard input
			tcsetattr(0, TCSANOW, &stored_settings);
		#endif
			return 0;

	}
}
