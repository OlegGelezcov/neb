import sys
from socket import *
import pickle

server_host = 'localhost'
server_port = 50008

class UserStat:
    def __init__(self, document):
        self.login = document['login']
        self.platform = document['platform']
        self.sessions = document['sessions']

    def session_count(self):
        if self.sessions is not None:
            return len(self.sessions)
        else:
            return 0

    def session_count_after(self, time_point):
        if self.sessions is not None:
            return len([time for time in self.sessions if time >= time_point])
        return 0

    def __str__(self):
        return 'login: {0}, platform: {1}, session count: {2}'.format(self.login, self.platform, self.session_count())


def on_stats_received(stats):
    for stat in stats:
        print(stat)


sockobj = socket(AF_INET, SOCK_STREAM)
sockobj.connect((server_host, server_port))

line = ''

while line != 'quit':
    line = input('Enter command: ')
    sockobj.send(line.encode('utf-8'))

    received_data = b''

    while True:
        data = sockobj.recv(1024)
        print('received', len(data), ' bytes')
        if not data:
            print('breaked')
            break
        else:
            received_data += data
            if len(data) < 1024:
                break


    if received_data:
        if line == 'total':
            stat_collection = pickle.loads(received_data)
            stats = []
            for document in stat_collection:
                stats.append(UserStat(document))
            print('stats received:', len(stats))
            on_stats_received(stats)

sockobj.close()

