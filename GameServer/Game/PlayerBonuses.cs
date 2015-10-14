using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Common;

namespace Space.Game
{

    public interface IBonuses
    {
        Bonus GetBonus(BonusType type);
        void ReplaceBuff(BonusType type, string id, Buff buff);
        void ReplaceBuff(BonusType type, string id, Buff buff, Notification replaceBuffNotif, Notification removeBuffNotif);
        void RemoveBuff(BonusType type, string id);
        void AddNotification(BonusType type, Notification notif);
        void RemoveNotification(BonusType type, Notification notif);
        float GetValue(BonusType type);

    }

    public delegate void Notification(float power);
    public interface IBonus
    {
        void ReplaceBuff(string id, Buff buff);
        void ReplaceBuff(string id, Buff buff, Notification replaceBuffNotif, Notification removeBuffNotif);
        void RemoveBuff(string id);
        void RemoveInvalidBuffs();
        void AddNotification(Notification notif);
        void RemoveNotification(Notification notif);
        float getValue { get; }
    }

    public delegate bool CheckBuff();
    public interface IBuff
    {
        bool Check();
        float power { get; }
        NotificationController ReplaceBuffNotifications { get; }
        NotificationController RemoveBuffNotifications { get; }
    }

    public interface INotificationController
    {
        void Add(Notification notif);
        void Remove(Notification notif);
        void Send(float value);
    }


    public class PlayerBonuses : IBonuses
    {
        private Dictionary<BonusType, Bonus> bonuses;
        
        public PlayerBonuses()
        {
            bonuses = new Dictionary<BonusType, Bonus>();

            var BonusTypeList = Enum.GetValues(typeof(BonusType)).Cast<BonusType>().ToList();

            BonusTypeList.ForEach((bt) =>
            {
                bonuses.Add(bt, new Bonus());
            });
        }


        public bool Contains(string buff_id)
        {
            foreach (var bonus in bonuses)
            {
                if (bonus.Value.Contains(buff_id))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(BonusType bonus_type, string buff_id)
        {
            return bonuses[bonus_type].Contains(buff_id);
        }

        public Bonus GetBonus(BonusType type)
        {
            if (bonuses.ContainsKey(type))
            {
                return bonuses[type];
            }
            return null;
        }

        public void ReplaceBuff(BonusType type, string id, Buff buff)
        {
            Bonus bonus = GetBonus(type);
            if (bonus != null)
            {
                bonus.ReplaceBuff(id, buff);
            }
        }
        public void ReplaceBuff(BonusType type, string id, Buff buff, Notification replaceBuffNotif, Notification removeBuffNotif)
        {
            Bonus bonus = GetBonus(type);
            if (buff.ReplaceBuffNotifications != null)
            {
                buff.ReplaceBuffNotifications.Add(replaceBuffNotif);
            }
            if (buff.RemoveBuffNotifications != null)
            {
                buff.RemoveBuffNotifications.Add(removeBuffNotif);
            }
            if (bonus != null)
            {
                bonus.ReplaceBuff(id, buff);
            }
        }

        public void RemoveBuff(BonusType type, string id)
        {
            Bonus bonus = GetBonus(type);
            if (bonus != null)
            {
                bonus.RemoveBuff(id);
            }
        }
        public void AddNotification(BonusType type, Notification notif)
        {
            Bonus bonus = GetBonus(type);
            if (bonus != null)
            {
                bonus.AddNotification(notif);
            }
        }
        public void RemoveNotification(BonusType type, Notification notif)
        {
            Bonus bonus = GetBonus(type);
            if (bonus != null)
            {
                bonus.RemoveNotification(notif);
            }
        }
        public float GetValue(BonusType type)
        {
            Bonus bonus = GetBonus(type);
            if (bonus != null)
            {
                return bonus.getValue;
            }
            return 0;
        }

        public Dictionary<BonusType, Bonus> Bonuses {
            get {
                return bonuses;
            }
        }
    }

    public class Bonus : IBonus
    {
        private Dictionary<string, Buff> _buffs;
        private List<string> _removeBuffs;
        private NotificationController _notifications;

        public Bonus()
        {
            _buffs = new Dictionary<string, Buff>();
            _removeBuffs = new List<string>();
            _notifications = new NotificationController();
        }

        public void ReplaceBuff(string id, Buff buff)
        {
            if (_buffs.ContainsKey(id))
            {
                _buffs[id].ReplaceBuffNotifications.Send(_buffs[id].power);
                _buffs[id] = buff;
            }
            else
            {
                _buffs.Add(id, buff);
            }
            _notifications.Send(getValue);
        }

        public void ReplaceBuff(string id, Buff buff, Notification replaceBuffNotif, Notification removeBuffNotif)
        {
            buff.ReplaceBuffNotifications.Add(replaceBuffNotif);
            buff.RemoveBuffNotifications.Add(removeBuffNotif);
            if (_buffs.ContainsKey(id))
            {
                _buffs[id].ReplaceBuffNotifications.Send(_buffs[id].power);
                _buffs[id] = buff;
            }
            else
            {
                _buffs.Add(id, buff);
            }
            _notifications.Send(getValue);
        }

        public void RemoveBuff(string id)
        {
            if (_buffs.ContainsKey(id))
            {
                _buffs[id].RemoveBuffNotifications.Send(_buffs[id].power);
                _buffs.Remove(id);
            }
            _notifications.Send(getValue);
        }

        public void RemoveInvalidBuffs()
        {
            _removeBuffs.ForEach((id) =>
            {
                if (_buffs.ContainsKey(id))
                {
                    _buffs[id].RemoveBuffNotifications.Send(_buffs[id].power);
                    _buffs.Remove(id);
                }
            });
            _removeBuffs.Clear();
        }

        public float getValue
        {
            get
            {
                float value = 0;

                foreach (var b in _buffs)
                {
                    if (b.Value.Check())
                    {
                        value += b.Value.power;
                    }
                    else
                    {
                        _removeBuffs.Add(b.Key);
                    }
                }

                if (_removeBuffs.Count > 0)
                {
                    _notifications.Send(value);
                }

                RemoveInvalidBuffs();

                return value;
            }
        }

        public bool HasAnyBuffs() {
            var val = getValue;
            return _buffs.Count != 0;
        }


        public void AddNotification(Notification notif)
        {
            _notifications.Add(notif);
        }
        public void RemoveNotification(Notification notif)
        {
            _notifications.Remove(notif);
        }

        public bool Contains(string buff_id)
        {
            return _buffs.ContainsKey(buff_id);
        }

    }

    public class Buff : IBuff
    {

        private CheckBuff _checkBuff;
        private float _power;
        private NotificationController _replaceNotifications = new NotificationController();
        private NotificationController _removeNotifications = new NotificationController();

        public Buff(float value)
        {
            _power = value;
            _checkBuff = () => { return true; };
        }

        public Buff(float value, CheckBuff check)
        {
            _power = value;
            _checkBuff = check;
        }

        public bool Check()
        {
            if (_checkBuff != null && _checkBuff())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public float power
        {
            get
            {
                return _power;
            }
        }

        public NotificationController ReplaceBuffNotifications
        {
            get
            {
                return _replaceNotifications;
            }
        }
        public NotificationController RemoveBuffNotifications
        {
            get
            {
                return _removeNotifications;
            }
        }
    }


    public class NotificationController : INotificationController
    {
        private event Notification _notifications;
        public void Add(Notification notif)
        {
            _notifications += notif;
        }
        public void Remove(Notification notif)
        {
            _notifications -= notif;
        }
        public void Send(float value)
        {
            if (_notifications != null)
            {
                _notifications(value);
            }
        }
    }



}
