// WeaponHitInfo.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Thursday, December 11, 2014 1:34:50 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//
namespace Space.Game {
    using Common;
    using ServerClientCommon;
    using System.Collections;

    public class WeaponHitInfo : IInfoSource
    {
        public  float hitProb;
        public  bool isHitted;
        public  bool hitAllowed;
        public  bool isCritical;

        private float actualDamage;
        private bool gunsOverheatted;
        private bool weaponBlocked;
        private string errorMessageId;

        public float remainTargetHP { get; private set; }

        public WeaponHitInfo() {
            hitAllowed = false;
            hitProb = 0f;
            isHitted = false;
            isCritical = false;
            errorMessageId = string.Empty;
        }

        public WeaponHitInfo(bool hitAllowed, float hitProb, bool isHitted, bool isCritical, string errorMsgId)
        {
            this.hitAllowed = hitAllowed;
            this.hitProb = hitProb;
            this.isHitted = isHitted;
            this.isCritical = isCritical;
            this.errorMessageId = errorMsgId;
        }

        public void SetActualDamage(float actualDamage)
        {
            this.actualDamage = actualDamage;
        }

        public void SetGunsOverheatted(bool overheatted)
        {
            this.gunsOverheatted = overheatted;
        }

        public void SetWeaponBlocked(bool weaponBlocked)
        {
            this.weaponBlocked = weaponBlocked;
        }

        public float ActualDamage
        {
            get { return this.actualDamage; }
        }

        public bool GunsOverheatted
        {
            get { return this.gunsOverheatted; }
        }

        public bool IsWeaponBlocked
        {
            get { return this.weaponBlocked; }
        }

        public bool notBlocked {
            get {
                return (!IsWeaponBlocked);
            }
        }

        public string ErrorMessageId
        {
            get
            {
                return this.errorMessageId;
            }
        }

        public void SetErrorMessageId(string errorMessageId)
        {
            this.errorMessageId = errorMessageId;
        }

        public void SetRemainTargetHp(float hp) {
            remainTargetHP = hp;
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.IsHitted, isHitted },
                { (int)SPC.ActualDamage, ActualDamage},
                { (int)SPC.OverheatingGuns, GunsOverheatted},
                { (int)SPC.FireBlocked, IsWeaponBlocked},
                { (int)SPC.FireAllowed, hitAllowed},
                { (int)SPC.IsCritical, isCritical },
                { (int)SPC.HitProb, hitProb},
                { (int)SPC.ErrorMessageId, ErrorMessageId}
            };
        }
    }
}
