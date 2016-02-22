namespace Common {
    public enum InventoryObjectType : byte
    {
        Weapon = 0,
        Scheme = 1,
        Material = 2,
        DrillScheme = 3, //scheme for setting drill in space
        Module = 4,
        None = 5,
        personal_beacon = 6,
        repair_kit = 7,
        repair_patch = 8,
        fort_upgrade = 9,
        out_upgrade = 10,
        turret = 11,
        fortification = 12,
        outpost = 13,
        mining_station = 14,
        nebula_element = 15,
        exp_boost = 16,
        loot_box = 17,
        
        //unused
        UNUSED_1 = 18,

        pet_scheme = 19,
        //unused
        UNUSED_2 = 20,

        craft_resource = 21,
        pet_skin = 22,
        founder_cube = 23,
        contract_item = 24,

        //pass elenment
        //pass
    }
}
