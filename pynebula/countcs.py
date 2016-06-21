import sys, os, pprint
trace = False
visited = {}
alllines = []
filecount = 0

if len(sys.argv) < 2:
    print('input path to directory as parameter')
else:
    if len(sys.argv) >= 3:
        print(sys.argv[2])

    for (thisDir, subsHere, filesHere) in os.walk(sys.argv[1]):
        if trace:
            print(thisDir)
        thisDir = os.path.normpath(thisDir)
        fixcase = os.path.normcase(thisDir)
        if fixcase in visited:
            continue
        else:
            visited[fixcase] = True
            for filename in filesHere:
                if filename.endswith('.cs') or filename.endswith('.py') or filename.endswith('.cpp') or filename.endswith('.h') or filename.endswith('.xml'):
                    filecount += 1
                    cspath = os.path.join(thisDir, filename)
                    try:
                        cslines = len(open(cspath, 'rb').readlines())
                        alllines.append((cslines, cspath))
                    except:
                        print('skipping', cspath, sys.exc_info()[0])

print('file count: ', filecount)
print('total lines: ', sum([ln for (ln, pt) in alllines]))
