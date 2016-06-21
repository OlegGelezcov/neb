import glob
import sys
import codecs
from xml.etree.ElementTree import parse

#sys.stdout = codecs.getwriter('utf8')(sys.stdout)

def uprint(*objects, sep=' ', file=sys.stdout):
    enc = file.encoding
    if enc == 'UTF-8':
        print(*objects, sep=sep, end=end, file=file)
    else:
        f = lambda obj: str(obj).encode(enc, errors='backslashreplace').decode(enc)
        print(*map(f, objects), sep=sep, file=file)



class StringData:
    def __init__(self, key, en):
        self.key = key
        self.en = en

    def __str__(self):
        return '{0}=>\n{1}en={2}'.format(self.key, ' ' * 4, self.en)

    def test_print(self):
        uprint(self.key, '\n', ' ' * 4, 'en=', self.en, sep='')

    def keyContains(self, text):
        return (text.lower() in self.key.lower())

    def valueContains(self, text):
        return (text.lower() in self.en.lower())


class StringFile:
    def __init__(self, name, strings):
        self.name = name
        self.strings = strings

    def __str__(self):
        return self.name + '\n' + '\n'.join([str(s) for s in self.strings])

    def test_print(self):
        uprint(self.name)
        for s in self.strings:
            s.test_print()

    def findKeys(self, key):
        return [s for s in self.strings if s.keyContains(key)]

    def findValues(self, value):
        return [s for s in self.strings if s.valueContains(value) ]



class StringFileCollection:
    def __init__(self):
        self.files = {}
    def addFile(self, file):
        self.files[file.name] = file
    def find(self, text):
        result = {}
        result['keys'] = []
        result['values'] = []
        for (name, file) in self.files.items():
            keyMatches = file.findKeys(text)
            if len(keyMatches) > 0:
                result['keys'].append((name, keyMatches))
            valueMatches = file.findValues(text)
            if len(valueMatches) > 0:
                result['values'].append((name, valueMatches))
        return result



    
def parseStringFile(file):
    doc = parse(file)
    strings = [StringData(item.attrib['key'], item.attrib['en']) for item in doc.iterfind('string')]
    return StringFile(file, strings)


def loadFiles():
    files = glob.glob(r'C:\Users\Dev\Documents\Nebula_PC\Assets\Resources\DataClient\Strings\*.xml')
    fileCollection = StringFileCollection()
    for file in files:
        file_data = parseStringFile(file)
        fileCollection.addFile(file_data)
    return fileCollection

     
def printFindResult(result):
    keyMatches = result['keys']
    valueMatches = result['values']

    print('KEYS==>')
    for (name, matches) in keyMatches:
        print('FILE: ', name, '==>')
        for m in matches:
            print(m)
        print()

    print('\n')
    print('VALUES==>')
    for (name, matches) in valueMatches:
        print('FILE: ', name, '==>')
        for m in matches:
            print(m)
        print()

#files = glob.glob(r'C:\Users\Dev\Documents\Nebula_PC\Assets\Resources\DataClient\Strings\*.xml')
#for file in files:
#    fileData = parseStringFile(file)
#    print(fileData)
#    print('-' * 20 )


if __name__=='__main__':
    fileCollection = loadFiles()
    exit = False

    while not exit:
        command = input('Enter command:')
        if command == 'quit' or command == 'exit':
            exit = True
        elif command.lower().strip().startswith('find'):
            tokens = command.strip().lower().split()
            tokens = [tok for tok in tokens if len(tok.strip()) > 0]
            if len(tokens) > 1:
                result = fileCollection.find(tokens[1])
                printFindResult(result)
            else:
                print('some error in command "find"')






