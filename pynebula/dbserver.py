from socket import *
from pymongo import *
import pickle

my_host = ''
my_port = 50008

dbclient = MongoClient("mongodb://localhost:27017")
db = dbclient.user_logins

sockobj = socket(AF_INET, SOCK_STREAM)
sockobj.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
sockobj.bind((my_host, my_port))
sockobj.listen(5)

try:
    while True:
        try:
            connection, address = sockobj.accept()
            print('Server connected by', address)

            client_data = b''
            while True:
                data = connection.recv(1024)
                if not data:
                    break
                else:
                    client_data += data


                # connection.send(b'Echo=>' + data)
                client_message = client_data.decode('utf-8')
                print('client data:', client_message)
                client_data = b''
                if client_message == 'total':
                    cursor = db.userstats.find()
                    stats = [document for document in cursor]
                    binary_data = pickle.dumps(stats)
                    connection.send(binary_data)
                    print('sended to client: ', binary_data)
                else:
                    connection.send(b'Unsupported command:' + client_data)

            connection.close()
        except ConnectionResetError as e:
            print('connection reset error')

except KeyboardInterrupt as e:
    print('keyboard interrupt occured')
    sockobj.close()


