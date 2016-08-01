import os, zipfile
from os.path import join
from os.path import relpath

def printdir(path):
    for root, dirs, files in os.walk(path):
        try:
            print(root)
        except:
            print('INVALID ROOT: ', root.encode())
            input('Press to continue')

        for dir in dirs:
            try:
                print(' ' * 8, 'DIR:', relpath(join(root, dir), path))
            except:
                print(' ' * 8, 'INVALID DIR:', relpath(join(root, dir), path).encode())
                input('Press to continue')

        for file in files:
            try:
                print(' ' * 8, 'FILE:', relpath(join(root, file), path))
            except:
                print(' ' * 8, 'INVALID FILE:', relpath(join(root, file), path).encode())
                input('Press to continue')


def log(msg):
    try:
        print(msg)
    except:
        print(msg.encode())


def zipfolder(dirname, zipfilename, includeEmptyDir=True):
    empty_dirs = []
    zip = zipfile.ZipFile(zipfilename, 'w', zipfile.ZIP_DEFLATED)
    for root, dirs, files in os.walk(dirname):
        empty_dirs.extend([dir for dir in dirs if os.listdir(join(root, dir)) == []])
        for name in files:
            try:
                rp = relpath(join(root, name), dirname)
                zip.write(join(root, name))
                log(rp)
            except:
                print('Exception when writing:', name.encode())
                input('Press any key')

        if includeEmptyDir:
            for dir in empty_dirs:
                try:
                    zif = zipfile.ZipInfo(join(root, dir) + '\\')
                    zip.writestr(zif, '')
                    log(relpath(join(root, dir), dirname))
                except:
                    print('Exception when writing:', name.encode())
                    input('Press any key')

        empty_dirs.clear()

    zip.close()


if __name__=='__main__':
    #printdir(r'C:\Users\Dev\Documents\Nebula_PC')
    zipfolder(r'C:\Users\Dev\Documents\Nebula_PC', r'C:\Users\Dev\Documents\Nebula.zip')


