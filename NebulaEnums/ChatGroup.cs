namespace Common
{
    public enum ChatGroup : int { 
        //players in current zone
        zone,
        //players in current group
        group,
        //players from current alliance
        guild,
        //only me
        whisper,
        //local message don't handled by server
        local,
        race
    }
}

