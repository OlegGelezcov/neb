// AppVersion.cs
// Nebula
//
// Created by Oleg Zheleztsov on Wednesday, September 16, 2015 8:16:44 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula {
    public class AppVersion {
        public int major { get; private set; }
        public int minor { get; private set; }
        public int revision { get; private set; }
        public int build { get; private set; }

        public AppVersion(int inMajor, int inMinor, int inRevision, int inBuild) {
            major = inMajor;
            minor = inMinor;
            revision = inRevision;
            build = inBuild;
        }

        /// <summary>
        /// Convert version to string
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("{0}.{1}.{2}.{3}", major, minor, revision, build);
        }

        /// <summary>
        /// Define compatible or not other version with this version
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Compatible(AppVersion other) {
            if(major < other.major) {
                return false;
            } else if(major > other.major) {
                return true;
            }

            if(minor < other.minor) {
                return false; 
            } else if(minor > other.minor) {
                return true;
            }

            if(revision < other.revision) {
                return false;
            } else if(revision > other.revision) {
                return true;
            }

            if(build < other.build) {
                return false;
            } else {
                return true;
            }
        }

        /// <summary>
        /// Return new version object from version string
        /// </summary>
        /// <param name="versionString"></param>
        /// <returns></returns>
        public static AppVersion FromString(string versionString ) {
            string[] tokens = versionString.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
            int major = 0, minor = 0, revision = 0, build = 0;
            if(tokens.Length > 0 ) {
                if(!int.TryParse(tokens[0], out major)) {
                    major = 0;
                }
            }
            if(tokens.Length > 1) {
                if(!int.TryParse(tokens[1], out minor)) {
                    minor = 0;
                }
            }
            if(tokens.Length > 2) {
                if(!int.TryParse(tokens[2], out revision)) {
                    revision = 0;
                }
            }
            if(tokens.Length > 3) {
                if(!int.TryParse(tokens[3], out build)) {
                    build = 0;
                }
            }
            return new AppVersion(major, minor, revision, build);
        }
    }
}
