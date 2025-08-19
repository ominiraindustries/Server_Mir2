# Mir2 Crystal OpCodes Detallados

Origen: [Shared/Enums.cs](cci:7://file:///c:/Users/Usuario/Desktop/Servidor%20Crystal%20Mir2/Shared/Enums.cs:0:0-0:0), [Shared/ServerPackets.cs](cci:7://file:///c:/Users/Usuario/Desktop/Servidor%20Crystal%20Mir2/Shared/ServerPackets.cs:0:0-0:0), [Shared/ClientPackets.cs](cci:7://file:///c:/Users/Usuario/Desktop/Servidor%20Crystal%20Mir2/Shared/ClientPackets.cs:0:0-0:0)

---

## Server  Client

### [0] Connected  clase Connected
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [1] ClientVersion  clase ClientVersion
- Dirección: Server
- Serialización (WritePacket): Result
- Deserialización (ReadPacket): ReadByte

### [2] Disconnect  clase Disconnect
- Dirección: Server
- Serialización (WritePacket): Reason
- Deserialización (ReadPacket): ReadByte

### [3] KeepAlive  clase KeepAlive
- Dirección: Server
- Serialización (WritePacket): Time
- Deserialización (ReadPacket): ReadInt64

### [4] NewAccount  clase NewAccount
- Dirección: Server
- Serialización (WritePacket): Result
- Deserialización (ReadPacket): ReadByte

### [5] ChangePassword  clase ChangePassword
- Dirección: Server
- Serialización (WritePacket): Result
- Deserialización (ReadPacket): ReadByte

### [6] ChangePasswordBanned  clase ChangePasswordBanned
- Dirección: Server
- Serialización (WritePacket): Reason, ExpiryDate.ToBinary()
- Deserialización (ReadPacket): ReadString

### [7] Login  clase Login
- Dirección: Server
- Serialización (WritePacket): Result
- Deserialización (ReadPacket): ReadByte

### [8] LoginBanned  clase LoginBanned
- Dirección: Server
- Serialización (WritePacket): Reason, ExpiryDate.ToBinary()
- Deserialización (ReadPacket): ReadString

### [9] LoginSuccess  clase LoginSuccess
- Dirección: Server
- Serialización (WritePacket): Characters.Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, [Nested new(..., reader)]

### [10] NewCharacter  clase NewCharacter
- Dirección: Server
- Serialización (WritePacket): Result
- Deserialización (ReadPacket): ReadByte

### [11] NewCharacterSuccess  clase NewCharacterSuccess
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [12] DeleteCharacter  clase DeleteCharacter
- Dirección: Server
- Serialización (WritePacket): Result
- Deserialización (ReadPacket): ReadByte

### [13] DeleteCharacterSuccess  clase DeleteCharacterSuccess
- Dirección: Server
- Serialización (WritePacket): CharacterIndex
- Deserialización (ReadPacket): ReadInt32

### [14] StartGame  clase StartGame
- Dirección: Server
- Serialización (WritePacket): Result, Resolution
- Deserialización (ReadPacket): ReadByte, ReadInt32

### [15] StartGameBanned  clase StartGameBanned
- Dirección: Server
- Serialización (WritePacket): Reason, ExpiryDate.ToBinary()
- Deserialización (ReadPacket): ReadString

### [16] StartGameDelay  clase StartGameDelay
- Dirección: Server
- Serialización (WritePacket): Milliseconds
- Deserialización (ReadPacket): ReadInt64

### [17] MapInformation  clase MapInformation
- Dirección: Server
- Serialización (WritePacket): MapIndex, FileName, Title, MiniMap, BigMap, (byte)Lights, bools, MapDarkLight, Music
- Deserialización (ReadPacket): ReadInt32, ReadString, ReadString, ReadUInt16, ReadUInt16, ReadByte, ReadByte, ReadUInt16

### [18] NewMapInfo  clase NewMapInfo
- Dirección: Server
- Serialización (WritePacket): MapIndex, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, [Nested new(..., reader)]

### [19] WorldMapSetup  clase WorldMapSetupInfo
- Dirección: Server
- Serialización (WritePacket): TeleportToNPCCost, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, [Nested new(..., reader)]

### [20] SearchMapResult  clase SearchMapResult
- Dirección: Server
- Serialización (WritePacket): MapIndex, NPCIndex
- Deserialización (ReadPacket): ReadInt32, ReadUInt32

### [21] UserInformation  clase UserInformation
- Dirección: Server
- Serialización (WritePacket): ObjectID, RealId, Name, GuildName, GuildRank, NameColour.ToArgb(), (byte)Class, (byte)Gender, Level, Location.X, Location.Y, (byte)Direction, Hair, HP, MP, Experience, MaxExperience, (ushort)LevelEffects, HasHero, (byte)HeroBehaviour, Inventory != null, Inventory.Length, Inventory[i] != null, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadUInt32, ReadUInt32, ReadString, ReadString, ReadString, ReadUInt16, ReadByte, ReadInt32, ReadInt32, ReadInt64, ReadInt64, ReadBoolean, [Nested new(..., reader)]

### [22] UserSlotsRefresh  clase UserSlotsRefresh
- Dirección: Server
- Serialización (WritePacket): Inventory != null, Inventory.Length, Inventory[i] != null, [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [23] UserLocation  clase UserLocation
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [24] ObjectPlayer  clase ObjectPlayer
- Dirección: Server
- Serialización (WritePacket): ObjectID, Name, GuildName, GuildRankName, NameColour.ToArgb(), (byte)Class, (byte)Gender, Level, Location.X, Location.Y, (byte)Direction, Hair, Light, Weapon, WeaponEffect, Armour, (ushort)Poison, Dead, Hidden, (byte)Effect, WingEffect, Extra, MountType, RidingMount, Fishing, TransformType, ElementOrbEffect, ElementOrbLvl, ElementOrbMax, Buffs.Count, (byte)Buffs[i]
- Deserialización (ReadPacket): ReadUInt32, ReadString, ReadString, ReadString, ReadUInt16, ReadByte, ReadByte, ReadInt16, ReadInt16, ReadInt16, ReadBoolean, ReadBoolean, ReadByte, ReadBoolean, ReadInt16, ReadBoolean, ReadBoolean, ReadInt16, ReadUInt32, ReadUInt32, ReadUInt32, ReadInt32

### [26] ObjectRemove  clase ObjectRemove
- Dirección: Server
- Serialización (WritePacket): ObjectID
- Deserialización (ReadPacket): ReadUInt32

### [27] ObjectTurn  clase ObjectTurn
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): ReadUInt32

### [28] ObjectWalk  clase ObjectWalk
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): ReadUInt32

### [29] ObjectRun  clase ObjectRun
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): ReadUInt32

### [30] Chat  clase Chat
- Dirección: Server
- Serialización (WritePacket): Message, (byte)Type
- Deserialización (ReadPacket): ReadString

### [31] ObjectChat  clase ObjectChat
- Dirección: Server
- Serialización (WritePacket): ObjectID, Text, (byte)Type
- Deserialización (ReadPacket): ReadUInt32, ReadString

### [32] NewItemInfo  clase NewItemInfo
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [33] NewHeroInfo  clase NewHeroInfo
- Dirección: Server
- Serialización (WritePacket): StorageIndex, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, [Nested new(..., reader)]

### [34] NewChatItem  clase NewChatItem
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [35] MoveItem  clase MoveItem
- Dirección: Server
- Serialización (WritePacket): (byte)Grid, From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [36] EquipItem  clase EquipItem
- Dirección: Server
- Serialización (WritePacket): (byte)Grid, UniqueID, To, Success
- Deserialización (ReadPacket): ReadUInt64, ReadInt32, ReadBoolean

### [37] MergeItem  clase MergeItem
- Dirección: Server
- Serialización (WritePacket): (byte)GridFrom, (byte)GridTo, IDFrom, IDTo, Success
- Deserialización (ReadPacket): ReadUInt64, ReadUInt64, ReadBoolean

### [38] RemoveItem  clase RemoveItem
- Dirección: Server
- Serialización (WritePacket): (byte)Grid, UniqueID, To, Success
- Deserialización (ReadPacket): ReadUInt64, ReadInt32, ReadBoolean

### [39] RemoveSlotItem  clase RemoveSlotItem
- Dirección: Server
- Serialización (WritePacket): (byte)Grid, (byte)GridTo, UniqueID, To, Success
- Deserialización (ReadPacket): ReadUInt64, ReadInt32, ReadBoolean

### [40] TakeBackItem  clase TakeBackItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [41] StoreItem  clase StoreItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [42] SplitItem  clase SplitItem
- Dirección: Server
- Serialización (WritePacket): Item != null, (byte)Grid, [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [43] SplitItem1  clase SplitItem1
- Dirección: Server
- Serialización (WritePacket): (byte)Grid, UniqueID, Count, Success
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16, ReadBoolean

### [44] DepositRefineItem  clase DepositRefineItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [45] RetrieveRefineItem  clase RetrieveRefineItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [46] RefineCancel  clase RefineCancel
- Dirección: Server
- Serialización (WritePacket): Unlock
- Deserialización (ReadPacket): ReadBoolean

### [47] RefineItem  clase RefineItem
- Dirección: Server
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [48] DepositTradeItem  clase DepositTradeItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [49] RetrieveTradeItem  clase RetrieveTradeItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [50] UseItem  clase UseItem
- Dirección: Server
- Serialización (WritePacket): UniqueID, Success, (byte)Grid
- Deserialización (ReadPacket): ReadUInt64, ReadBoolean

### [51] DropItem  clase DropItem
- Dirección: Server
- Serialización (WritePacket): UniqueID, Count, HeroItem, Success
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16, ReadBoolean, ReadBoolean

### [52] TakeBackHeroItem  clase TakeBackHeroItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [53] TransferHeroItem  clase TransferHeroItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [54] PlayerUpdate  clase PlayerUpdate
- Dirección: Server
- Serialización (WritePacket): ObjectID, Light, Weapon, WeaponEffect, Armour, WingEffect
- Deserialización (ReadPacket): ReadUInt32, ReadByte, ReadInt16, ReadInt16, ReadInt16, ReadByte

### [55] PlayerInspect  clase PlayerInspect
- Dirección: Server
- Serialización (WritePacket): Name, GuildName, GuildRank, Equipment.Length, T != null, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadString, ReadString, ReadString, [Nested new(..., reader)]

### [56] LogOutSuccess  clase LogOutSuccess
- Dirección: Server
- Serialización (WritePacket): Characters.Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, [Nested new(..., reader)]

### [57] LogOutFailed  clase LogOutFailed
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [58] ReturnToLogin  clase ReturnToLogin
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [59] TimeOfDay  clase TimeOfDay
- Dirección: Server
- Serialización (WritePacket): (byte)Lights
- Deserialización (ReadPacket): (no detectada)

### [60] ChangeAMode  clase ChangeAMode
- Dirección: Server
- Serialización (WritePacket): (byte)Mode
- Deserialización (ReadPacket): (no detectada)

### [61] ChangePMode  clase ChangePMode
- Dirección: Server
- Serialización (WritePacket): (byte)Mode
- Deserialización (ReadPacket): (no detectada)

### [62] ObjectItem  clase ObjectItem
- Dirección: Server
- Serialización (WritePacket): ObjectID, Name, NameColour.ToArgb(), Location.X, Location.Y, Image, (byte)grade
- Deserialización (ReadPacket): ReadUInt32, ReadString, ReadUInt16

### [63] ObjectGold  clase ObjectGold
- Dirección: Server
- Serialización (WritePacket): ObjectID, Gold, Location.X, Location.Y
- Deserialización (ReadPacket): ReadUInt32, ReadUInt32

### [64] GainedItem  clase GainedItem
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [65] GainedGold  clase GainedGold
- Dirección: Server
- Serialización (WritePacket): Gold
- Deserialización (ReadPacket): ReadUInt32

### [66] LoseGold  clase LoseGold
- Dirección: Server
- Serialización (WritePacket): Gold
- Deserialización (ReadPacket): ReadUInt32

### [67] GainedCredit  clase GainedCredit
- Dirección: Server
- Serialización (WritePacket): Credit
- Deserialización (ReadPacket): ReadUInt32

### [68] LoseCredit  clase LoseCredit
- Dirección: Server
- Serialización (WritePacket): Credit
- Deserialización (ReadPacket): ReadUInt32

### [69] ObjectMonster  clase ObjectMonster
- Dirección: Server
- Serialización (WritePacket): ObjectID, Name, NameColour.ToArgb(), Location.X, Location.Y, (ushort)Image, (byte)Direction, Effect, AI, Light, Dead, Skeleton, (ushort)Poison, Hidden, ShockTime, BindingShotCenter, Extra, (byte)ExtraByte, Buffs.Count, (byte)Buffs[i]
- Deserialización (ReadPacket): ReadUInt32, ReadString, ReadByte, ReadByte, ReadByte, ReadBoolean, ReadBoolean, ReadBoolean, ReadInt64, ReadBoolean, ReadBoolean, ReadByte, ReadInt32

### [70] ObjectAttack  clase ObjectAttack
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction, (byte)Spell, Level, Type
- Deserialización (ReadPacket): ReadUInt32, ReadByte, ReadByte

### [71] Struck  clase Struck
- Dirección: Server
- Serialización (WritePacket): AttackerID
- Deserialización (ReadPacket): ReadUInt32

### [72] ObjectStruck  clase ObjectStruck
- Dirección: Server
- Serialización (WritePacket): ObjectID, AttackerID, Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): ReadUInt32, ReadUInt32

### [73] DamageIndicator  clase DamageIndicator
- Dirección: Server
- Serialización (WritePacket): Damage, (byte)Type, ObjectID
- Deserialización (ReadPacket): ReadInt32, ReadUInt32

### [74] DuraChanged  clase DuraChanged
- Dirección: Server
- Serialización (WritePacket): UniqueID, CurrentDura
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16

### [75] HealthChanged  clase HealthChanged
- Dirección: Server
- Serialización (WritePacket): HP, MP
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [76] HeroHealthChanged  clase HeroHealthChanged
- Dirección: Server
- Serialización (WritePacket): HP, MP
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [77] DeleteItem  clase DeleteItem
- Dirección: Server
- Serialización (WritePacket): UniqueID, Count
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16

### [78] Death  clase Death
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [79] ObjectDied  clase ObjectDied
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction, Type
- Deserialización (ReadPacket): ReadUInt32, ReadByte

### [80] ColourChanged  clase ColourChanged
- Dirección: Server
- Serialización (WritePacket): NameColour.ToArgb()
- Deserialización (ReadPacket): (no detectada)

### [81] ObjectColourChanged  clase ObjectColourChanged
- Dirección: Server
- Serialización (WritePacket): ObjectID, NameColour.ToArgb()
- Deserialización (ReadPacket): ReadUInt32

### [82] ObjectGuildNameChanged  clase ObjectGuildNameChanged
- Dirección: Server
- Serialización (WritePacket): ObjectID, GuildName
- Deserialización (ReadPacket): ReadUInt32, ReadString

### [83] GainExperience  clase GainExperience
- Dirección: Server
- Serialización (WritePacket): Amount
- Deserialización (ReadPacket): ReadUInt32

### [84] GainHeroExperience  clase GainHeroExperience
- Dirección: Server
- Serialización (WritePacket): Amount
- Deserialización (ReadPacket): ReadUInt32

### [85] LevelChanged  clase LevelChanged
- Dirección: Server
- Serialización (WritePacket): Level, Experience, MaxExperience
- Deserialización (ReadPacket): ReadUInt16, ReadInt64, ReadInt64

### [86] HeroLevelChanged  clase HeroLevelChanged
- Dirección: Server
- Serialización (WritePacket): Level, Experience, MaxExperience
- Deserialización (ReadPacket): ReadUInt16, ReadInt64, ReadInt64

### [87] ObjectLeveled  clase ObjectLeveled
- Dirección: Server
- Serialización (WritePacket): ObjectID
- Deserialización (ReadPacket): ReadUInt32

### [88] ObjectHarvest  clase ObjectHarvest
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): ReadUInt32

### [89] ObjectHarvested  clase ObjectHarvested
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): ReadUInt32

### [90] ObjectNpc  clase ObjectNPC
- Dirección: Server
- Serialización (WritePacket): ObjectID, Name, NameColour.ToArgb(), Image, Colour.ToArgb(), Location.X, Location.Y, (byte)Direction, QuestIDs.Count, QuestIDs[i]
- Deserialización (ReadPacket): ReadUInt32, ReadString, ReadUInt16, ReadInt32

### [91] NPCResponse  clase NPCResponse
- Dirección: Server
- Serialización (WritePacket): Page.Count, Page[i]
- Deserialización (ReadPacket): ReadInt32

### [92] ObjectHide  clase ObjectHide
- Dirección: Server
- Serialización (WritePacket): ObjectID
- Deserialización (ReadPacket): ReadUInt32

### [93] ObjectShow  clase ObjectShow
- Dirección: Server
- Serialización (WritePacket): ObjectID
- Deserialización (ReadPacket): ReadUInt32

### [94] Poisoned  clase Poisoned
- Dirección: Server
- Serialización (WritePacket): (ushort)Poison
- Deserialización (ReadPacket): (no detectada)

### [95] ObjectPoisoned  clase ObjectPoisoned
- Dirección: Server
- Serialización (WritePacket): ObjectID, (ushort)Poison
- Deserialización (ReadPacket): ReadUInt32

### [96] MapChanged  clase MapChanged
- Dirección: Server
- Serialización (WritePacket): MapIndex, FileName, Title, MiniMap, BigMap, (byte)Lights, Location.X, Location.Y, (byte)Direction, MapDarkLight, Music
- Deserialización (ReadPacket): ReadInt32, ReadString, ReadString, ReadUInt16, ReadUInt16, ReadByte, ReadUInt16

### [97] ObjectTeleportOut  clase ObjectTeleportOut
- Dirección: Server
- Serialización (WritePacket): ObjectID, Type
- Deserialización (ReadPacket): ReadUInt32, ReadByte

### [98] ObjectTeleportIn  clase ObjectTeleportIn
- Dirección: Server
- Serialización (WritePacket): ObjectID, Type
- Deserialización (ReadPacket): ReadUInt32, ReadByte

### [99] TeleportIn  clase TeleportIn
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [100] NPCGoods  clase NPCGoods
- Dirección: Server
- Serialización (WritePacket): List.Count, Rate, (byte)Type, HideAddedStats, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, ReadSingle, ReadBoolean, [Nested new(..., reader)]

### [101] NPCSell  clase NPCSell
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [102] NPCRepair  clase NPCRepair
- Dirección: Server
- Serialización (WritePacket): Rate
- Deserialización (ReadPacket): ReadSingle

### [103] NPCSRepair  clase NPCSRepair
- Dirección: Server
- Serialización (WritePacket): Rate
- Deserialización (ReadPacket): ReadSingle

### [104] NPCRefine  clase NPCRefine
- Dirección: Server
- Serialización (WritePacket): Rate, Refining
- Deserialización (ReadPacket): ReadSingle, ReadBoolean

### [105] NPCCheckRefine  clase NPCCheckRefine
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [106] NPCCollectRefine  clase NPCCollectRefine
- Dirección: Server
- Serialización (WritePacket): Success
- Deserialización (ReadPacket): ReadBoolean

### [107] NPCReplaceWedRing  clase NPCReplaceWedRing
- Dirección: Server
- Serialización (WritePacket): Rate
- Deserialización (ReadPacket): ReadSingle

### [108] NPCStorage  clase NPCStorage
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [109] SellItem  clase SellItem
- Dirección: Server
- Serialización (WritePacket): UniqueID, Count, Success
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16, ReadBoolean

### [110] CraftItem  clase CraftItem
- Dirección: Server
- Serialización (WritePacket): Success
- Deserialización (ReadPacket): ReadBoolean

### [111] RepairItem  clase RepairItem
- Dirección: Server
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [112] ItemRepaired  clase ItemRepaired
- Dirección: Server
- Serialización (WritePacket): UniqueID, MaxDura, CurrentDura
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16, ReadUInt16

### [113] ItemSlotSizeChanged  clase ItemSlotSizeChanged
- Dirección: Server
- Serialización (WritePacket): UniqueID, SlotSize
- Deserialización (ReadPacket): ReadUInt64, ReadInt32

### [114] ItemSealChanged  clase ItemSealChanged
- Dirección: Server
- Serialización (WritePacket): UniqueID, ExpiryDate.ToBinary()
- Deserialización (ReadPacket): ReadUInt64

### [115] NewMagic  clase NewMagic
- Dirección: Server
- Serialización (WritePacket): Hero, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadBoolean, [Nested new(..., reader)]

### [116] RemoveMagic  clase RemoveMagic
- Dirección: Server
- Serialización (WritePacket): PlaceId
- Deserialización (ReadPacket): ReadInt32

### [117] MagicLeveled  clase MagicLeveled
- Dirección: Server
- Serialización (WritePacket): ObjectID, (byte)Spell, Level, Experience
- Deserialización (ReadPacket): ReadUInt32, ReadByte, ReadUInt16

### [118] Magic  clase Magic
- Dirección: Server
- Serialización (WritePacket): (byte)Spell, TargetID, Target.X, Target.Y, Cast, Level, SecondaryTargetIDs.Count, targetID
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean, ReadByte, ReadInt32

### [119] MagicDelay  clase MagicDelay
- Dirección: Server
- Serialización (WritePacket): ObjectID, (byte)Spell, Delay
- Deserialización (ReadPacket): ReadUInt32, ReadInt64

### [120] MagicCast  clase MagicCast
- Dirección: Server
- Serialización (WritePacket): (byte)Spell
- Deserialización (ReadPacket): (no detectada)

### [121] ObjectMagic  clase ObjectMagic
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction, (byte)Spell, TargetID, Target.X, Target.Y, Cast, Level, SelfBroadcast, SecondaryTargetIDs.Count, targetID
- Deserialización (ReadPacket): ReadUInt32, ReadUInt32, ReadBoolean, ReadByte, ReadBoolean, ReadInt32

### [122] ObjectEffect  clase ObjectEffect
- Dirección: Server
- Serialización (WritePacket): ObjectID, (byte)Effect, EffectType, DelayTime, Time
- Deserialización (ReadPacket): ReadUInt32, ReadUInt32, ReadUInt32, ReadUInt32

### [123] ObjectProjectile  clase ObjectProjectile
- Dirección: Server
- Serialización (WritePacket): (byte)Spell, Source, Destination
- Deserialización (ReadPacket): ReadUInt32, ReadUInt32

### [124] RangeAttack  clase RangeAttack
- Dirección: Server
- Serialización (WritePacket): TargetID, Target.X, Target.Y, (byte)Spell
- Deserialización (ReadPacket): ReadUInt32

### [125] Pushed  clase Pushed
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [126] ObjectPushed  clase ObjectPushed
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): ReadUInt32

### [127] ObjectName  clase ObjectName
- Dirección: Server
- Serialización (WritePacket): ObjectID, Name
- Deserialización (ReadPacket): ReadUInt32, ReadString

### [128] UserStorage  clase UserStorage
- Dirección: Server
- Serialización (WritePacket): Storage != null, Storage.Length, Storage[i] != null, [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [129] SwitchGroup  clase SwitchGroup
- Dirección: Server
- Serialización (WritePacket): AllowGroup
- Deserialización (ReadPacket): ReadBoolean

### [130] DeleteGroup  clase DeleteGroup
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [131] DeleteMember  clase DeleteMember
- Dirección: Server
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [132] GroupInvite  clase GroupInvite
- Dirección: Server
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [133] AddMember  clase AddMember
- Dirección: Server
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [134] Revived  clase Revived
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [135] ObjectRevived  clase ObjectRevived
- Dirección: Server
- Serialización (WritePacket): ObjectID, Effect
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean

### [136] SpellToggle  clase SpellToggle
- Dirección: Server
- Serialización (WritePacket): ObjectID, (byte)Spell, CanUse
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean

### [137] ObjectHealth  clase ObjectHealth
- Dirección: Server
- Serialización (WritePacket): ObjectID, Percent, Expire
- Deserialización (ReadPacket): ReadUInt32, ReadByte, ReadByte

### [138] ObjectMana  clase ObjectMana
- Dirección: Server
- Serialización (WritePacket): ObjectID, Percent
- Deserialización (ReadPacket): ReadUInt32, ReadByte

### [139] MapEffect  clase MapEffect
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y, (byte)Effect, Value
- Deserialización (ReadPacket): ReadByte

### [140] AllowObserve  clase AllowObserve
- Dirección: Server
- Serialización (WritePacket): Allow
- Deserialización (ReadPacket): ReadBoolean

### [141] ObjectRangeAttack  clase ObjectRangeAttack
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction, TargetID, Target.X, Target.Y, Type, (byte)Spell, Level
- Deserialización (ReadPacket): ReadUInt32, ReadUInt32, ReadByte, ReadByte

### [142] AddBuff  clase AddBuff
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [143] RemoveBuff  clase RemoveBuff
- Dirección: Server
- Serialización (WritePacket): (byte)Type, ObjectID
- Deserialización (ReadPacket): ReadUInt32

### [144] PauseBuff  clase PauseBuff
- Dirección: Server
- Serialización (WritePacket): (byte)Type, ObjectID, Paused
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean

### [145] ObjectHidden  clase ObjectHidden
- Dirección: Server
- Serialización (WritePacket): ObjectID, Hidden
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean

### [146] RefreshItem  clase RefreshItem
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [147] ObjectSpell  clase ObjectSpell
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Spell, (byte)Direction, Param
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean

### [148] UserDash  clase UserDash
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [149] ObjectDash  clase ObjectDash
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): ReadUInt32

### [150] UserDashFail  clase UserDashFail
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [151] ObjectDashFail  clase ObjectDashFail
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): ReadUInt32

### [152] NPCConsign  clase NPCConsign
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [153] NPCMarket  clase NPCMarket
- Dirección: Server
- Serialización (WritePacket): Listings.Count, Pages, UserMode, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean, [Nested new(..., reader)]

### [154] NPCMarketPage  clase NPCMarketPage
- Dirección: Server
- Serialización (WritePacket): Listings.Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, [Nested new(..., reader)]

### [155] ConsignItem  clase ConsignItem
- Dirección: Server
- Serialización (WritePacket): UniqueID, Success
- Deserialización (ReadPacket): ReadUInt64, ReadBoolean

### [156] MarketFail  clase MarketFail
- Dirección: Server
- Serialización (WritePacket): Reason
- Deserialización (ReadPacket): ReadByte

### [157] MarketSuccess  clase MarketSuccess
- Dirección: Server
- Serialización (WritePacket): Message
- Deserialización (ReadPacket): ReadString

### [158] ObjectSitDown  clase ObjectSitDown
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction, Sitting
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean

### [159] InTrapRock  clase InTrapRock
- Dirección: Server
- Serialización (WritePacket): Trapped
- Deserialización (ReadPacket): ReadBoolean

### [160] BaseStatsInfo  clase BaseStatsInfo
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [161] HeroBaseStatsInfo  clase HeroBaseStatsInfo
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [162] UserName  clase UserName
- Dirección: Server
- Serialización (WritePacket): Id, Name
- Deserialización (ReadPacket): ReadUInt32, ReadString

### [163] ChatItemStats  clase ChatItemStats
- Dirección: Server
- Serialización (WritePacket): ChatItemId, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadUInt64, [Nested new(..., reader)]

### [164] GuildNoticeChange  clase GuildNoticeChange
- Dirección: Server
- Serialización (WritePacket): update
- Deserialización (ReadPacket): ReadInt32

### [165] GuildMemberChange  clase GuildMemberChange
- Dirección: Server
- Serialización (WritePacket): Name, RankIndex, Status, Ranks.Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadString, ReadByte, ReadByte, ReadInt32, [Nested new(..., reader)]

### [166] GuildStatus  clase GuildStatus
- Dirección: Server
- Serialización (WritePacket): GuildName, GuildRankName, Level, Experience, MaxExperience, Gold, SparePoints, MemberCount, MaxMembers, Voting, ItemCount, BuffCount, (byte)MyOptions, MyRankId
- Deserialización (ReadPacket): ReadString, ReadString, ReadByte, ReadInt64, ReadInt64, ReadUInt32, ReadByte, ReadInt32, ReadInt32, ReadBoolean, ReadByte, ReadByte, ReadInt32

### [167] GuildInvite  clase GuildInvite
- Dirección: Server
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [168] GuildExpGain  clase GuildExpGain
- Dirección: Server
- Serialización (WritePacket): Amount
- Deserialización (ReadPacket): ReadUInt32

### [169] GuildNameRequest  clase GuildNameRequest
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [170] GuildStorageGoldChange  clase GuildStorageGoldChange
- Dirección: Server
- Serialización (WritePacket): Amount, Type, Name
- Deserialización (ReadPacket): ReadUInt32, ReadByte, ReadString

### [171] GuildStorageItemChange  clase GuildStorageItemChange
- Dirección: Server
- Serialización (WritePacket): Type, To, From, User, Item != null, Item.UserId, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadByte, ReadInt32, ReadInt32, ReadInt32, [Nested new(..., reader)]

### [172] GuildStorageList  clase GuildStorageList
- Dirección: Server
- Serialización (WritePacket): Items.Length, Items[i] != null, [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [173] GuildRequestWar  clase GuildRequestWar
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [174] HeroCreateRequest  clase HeroCreateRequest
- Dirección: Server
- Serialización (WritePacket): CanCreateClass.Length, CanCreateClass[i]
- Deserialización (ReadPacket): ReadInt32, ReadBoolean

### [175] NewHero  clase NewHero
- Dirección: Server
- Serialización (WritePacket): Result
- Deserialización (ReadPacket): ReadByte

### [177] UpdateHeroSpawnState  clase UpdateHeroSpawnState
- Dirección: Server
- Serialización (WritePacket): (byte)State
- Deserialización (ReadPacket): (no detectada)

### [178] UnlockHeroAutoPot  clase UnlockHeroAutoPot
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [179] SetAutoPotValue  clase SetAutoPotValue
- Dirección: Server
- Serialización (WritePacket): (byte)Stat, Value
- Deserialización (ReadPacket): ReadUInt32

### [180] SetAutoPotItem  clase SetAutoPotItem
- Dirección: Server
- Serialización (WritePacket): (byte)Grid, ItemIndex
- Deserialización (ReadPacket): ReadInt32

### [181] SetHeroBehaviour  clase SetHeroBehaviour
- Dirección: Server
- Serialización (WritePacket): (byte)Behaviour
- Deserialización (ReadPacket): (no detectada)

### [182] ManageHeroes  clase ManageHeroes
- Dirección: Server
- Serialización (WritePacket): MaximumCount, CurrentHero != null, Heroes != null, Heroes.Length, Heroes[i] != null, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, ReadInt32, [Nested new(..., reader)]

### [183] ChangeHero  clase ChangeHero
- Dirección: Server
- Serialización (WritePacket): FromIndex
- Deserialización (ReadPacket): ReadInt32

### [184] DefaultNPC  clase DefaultNPC
- Dirección: Server
- Serialización (WritePacket): ObjectID
- Deserialización (ReadPacket): ReadUInt32

### [185] NPCUpdate  clase NPCUpdate
- Dirección: Server
- Serialización (WritePacket): NPCID
- Deserialización (ReadPacket): ReadUInt32

### [186] NPCImageUpdate  clase NPCImageUpdate
- Dirección: Server
- Serialización (WritePacket): ObjectID, Image, Colour.ToArgb()
- Deserialización (ReadPacket): ReadInt64, ReadUInt16

### [187] MarriageRequest  clase MarriageRequest
- Dirección: Server
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [188] DivorceRequest  clase DivorceRequest
- Dirección: Server
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [189] MentorRequest  clase MentorRequest
- Dirección: Server
- Serialización (WritePacket): Name, Level
- Deserialización (ReadPacket): ReadString, ReadUInt16

### [190] TradeRequest  clase TradeRequest
- Dirección: Server
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [191] TradeAccept  clase TradeAccept
- Dirección: Server
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [192] TradeGold  clase TradeGold
- Dirección: Server
- Serialización (WritePacket): Amount
- Deserialización (ReadPacket): ReadUInt32

### [193] TradeItem  clase TradeItem
- Dirección: Server
- Serialización (WritePacket): TradeItems.Length, T != null, [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [194] TradeConfirm  clase TradeConfirm
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [195] TradeCancel  clase TradeCancel
- Dirección: Server
- Serialización (WritePacket): Unlock
- Deserialización (ReadPacket): ReadBoolean

### [196] MountUpdate  clase MountUpdate
- Dirección: Server
- Serialización (WritePacket): ObjectID, MountType, RidingMount
- Deserialización (ReadPacket): ReadInt64, ReadInt16, ReadBoolean

### [197] EquipSlotItem  clase EquipSlotItem
- Dirección: Server
- Serialización (WritePacket): (byte)Grid, UniqueID, To, (byte)GridTo, Success
- Deserialización (ReadPacket): ReadUInt64, ReadInt32, ReadBoolean

### [198] FishingUpdate  clase FishingUpdate
- Dirección: Server
- Serialización (WritePacket): ObjectID, Fishing, ProgressPercent, ChancePercent, FishingPoint.X, FishingPoint.Y, FoundFish
- Deserialización (ReadPacket): ReadInt64, ReadBoolean, ReadInt32, ReadInt32, ReadBoolean

### [199] ChangeQuest  clase ChangeQuest
- Dirección: Server
- Serialización (WritePacket): (byte)QuestState, TrackQuest, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadBoolean, [Nested new(..., reader)]

### [200] CompleteQuest  clase CompleteQuest
- Dirección: Server
- Serialización (WritePacket): CompletedQuests.Count, q
- Deserialización (ReadPacket): ReadInt32

### [201] ShareQuest  clase ShareQuest
- Dirección: Server
- Serialización (WritePacket): QuestIndex, SharerName
- Deserialización (ReadPacket): ReadInt32, ReadString

### [202] NewQuestInfo  clase NewQuestInfo
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [203] GainedQuestItem  clase GainedQuestItem
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [204] DeleteQuestItem  clase DeleteQuestItem
- Dirección: Server
- Serialización (WritePacket): UniqueID, Count
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16

### [205] CancelReincarnation  clase CancelReincarnation
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [206] RequestReincarnation  clase RequestReincarnation
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [207] UserBackStep  clase UserBackStep
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [208] ObjectBackStep  clase ObjectBackStep
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction, Distance
- Deserialización (ReadPacket): ReadUInt32, ReadInt16

### [209] UserDashAttack  clase UserDashAttack
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [210] ObjectDashAttack  clase ObjectDashAttack
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, (byte)Direction, Distance
- Deserialización (ReadPacket): ReadUInt32, ReadInt16

### [211] UserAttackMove  clase UserAttackMove
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y, (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [212] CombineItem  clase CombineItem
- Dirección: Server
- Serialización (WritePacket): (byte)Grid, IDFrom, IDTo, Success, Destroy
- Deserialización (ReadPacket): ReadUInt64, ReadUInt64, ReadBoolean, ReadBoolean

### [213] ItemUpgraded  clase ItemUpgraded
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [214] SetConcentration  clase SetConcentration
- Dirección: Server
- Serialización (WritePacket): ObjectID, Enabled, Interrupted
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean, ReadBoolean

### [215] SetElemental  clase SetElemental
- Dirección: Server
- Serialización (WritePacket): ObjectID, Enabled, Casted, Value, ElementType, ExpLast
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean, ReadBoolean, ReadUInt32, ReadUInt32, ReadUInt32

### [216] RemoveDelayedExplosion  clase RemoveDelayedExplosion
- Dirección: Server
- Serialización (WritePacket): ObjectID
- Deserialización (ReadPacket): ReadUInt32

### [217] ObjectDeco  clase ObjectDeco
- Dirección: Server
- Serialización (WritePacket): ObjectID, Location.X, Location.Y, Image
- Deserialización (ReadPacket): ReadUInt32, ReadInt32

### [218] ObjectSneaking  clase ObjectSneaking
- Dirección: Server
- Serialización (WritePacket): ObjectID, SneakingActive
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean

### [219] ObjectLevelEffects  clase ObjectLevelEffects
- Dirección: Server
- Serialización (WritePacket): ObjectID, (ushort)LevelEffects
- Deserialización (ReadPacket): ReadUInt32

### [220] SetBindingShot  clase SetBindingShot
- Dirección: Server
- Serialización (WritePacket): ObjectID, Enabled, Value
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean, ReadInt64

### [221] SendOutputMessage  clase SendOutputMessage
- Dirección: Server
- Serialización (WritePacket): Message, (byte)Type
- Deserialización (ReadPacket): ReadString

### [222] NPCAwakening  clase NPCAwakening
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [223] NPCDisassemble  clase NPCDisassemble
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [224] NPCDowngrade  clase NPCDowngrade
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [225] NPCReset  clase NPCReset
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [226] AwakeningNeedMaterials  clase AwakeningNeedMaterials
- Dirección: Server
- Serialización (WritePacket): Materials != null, Materials.Length, Materials[i] != null, MaterialsCount[i], [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, ReadByte, [Nested new(..., reader)]

### [227] AwakeningLockedItem  clase AwakeningLockedItem
- Dirección: Server
- Serialización (WritePacket): UniqueID, Locked
- Deserialización (ReadPacket): ReadUInt64, ReadBoolean

### [228] Awakening  clase Awakening
- Dirección: Server
- Serialización (WritePacket): result, removeID
- Deserialización (ReadPacket): ReadInt32, ReadInt64

### [229] ReceiveMail  clase ReceiveMail
- Dirección: Server
- Serialización (WritePacket): Mail.Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, [Nested new(..., reader)]

### [230] MailLockedItem  clase MailLockedItem
- Dirección: Server
- Serialización (WritePacket): UniqueID, Locked
- Deserialización (ReadPacket): ReadUInt64, ReadBoolean

### [231] MailSendRequest  clase MailSendRequest
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [232] MailSent  clase MailSent
- Dirección: Server
- Serialización (WritePacket): Result
- Deserialización (ReadPacket): ReadSByte

### [233] ParcelCollected  clase ParcelCollected
- Dirección: Server
- Serialización (WritePacket): Result
- Deserialización (ReadPacket): ReadSByte

### [234] MailCost  clase MailCost
- Dirección: Server
- Serialización (WritePacket): Cost
- Deserialización (ReadPacket): ReadUInt32

### [235] ResizeInventory  clase ResizeInventory
- Dirección: Server
- Serialización (WritePacket): Size
- Deserialización (ReadPacket): ReadInt32

### [236] ResizeStorage  clase ResizeStorage
- Dirección: Server
- Serialización (WritePacket): Size, HasExpandedStorage, ExpiryTime.ToBinary()
- Deserialización (ReadPacket): ReadInt32, ReadBoolean

### [237] NewIntelligentCreature  clase NewIntelligentCreature
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [238] UpdateIntelligentCreatureList  clase UpdateIntelligentCreatureList
- Dirección: Server
- Serialización (WritePacket): CreatureList.Count, CreatureSummoned, (byte)SummonedCreatureType, PearlCount, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, ReadBoolean, ReadInt32, [Nested new(..., reader)]

### [239] IntelligentCreatureEnableRename  clase IntelligentCreatureEnableRename
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [240] IntelligentCreaturePickup  clase IntelligentCreaturePickup
- Dirección: Server
- Serialización (WritePacket): ObjectID
- Deserialización (ReadPacket): ReadUInt32

### [241] NPCPearlGoods  clase NPCPearlGoods
- Dirección: Server
- Serialización (WritePacket): List.Count, Rate, (byte)Type, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, ReadSingle, [Nested new(..., reader)]

### [242] TransformUpdate  clase TransformUpdate
- Dirección: Server
- Serialización (WritePacket): ObjectID, TransformType
- Deserialización (ReadPacket): ReadInt64, ReadInt16

### [243] FriendUpdate  clase FriendUpdate
- Dirección: Server
- Serialización (WritePacket): Friends.Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, [Nested new(..., reader)]

### [244] LoverUpdate  clase LoverUpdate
- Dirección: Server
- Serialización (WritePacket): Name, Date.ToBinary(), MapName, MarriedDays
- Deserialización (ReadPacket): ReadString, ReadString, ReadInt16

### [245] MentorUpdate  clase MentorUpdate
- Dirección: Server
- Serialización (WritePacket): Name, Level, Online, MenteeEXP
- Deserialización (ReadPacket): ReadString, ReadUInt16, ReadBoolean, ReadInt64

### [246] GuildBuffList  clase GuildBuffList
- Dirección: Server
- Serialización (WritePacket): Remove, ActiveBuffs.Count, GuildBuffs.Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadByte, ReadInt32, ReadInt32, [Nested new(..., reader)]

### [247] NPCRequestInput  clase NPCRequestInput
- Dirección: Server
- Serialización (WritePacket): NPCID, PageName
- Deserialización (ReadPacket): ReadUInt32, ReadString

### [248] GameShopInfo  clase GameShopInfo
- Dirección: Server
- Serialización (WritePacket): StockLevel
- Deserialización (ReadPacket): ReadInt32

### [249] GameShopStock  clase GameShopStock
- Dirección: Server
- Serialización (WritePacket): GIndex, StockLevel
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [250] Rankings  clase Rankings
- Dirección: Server
- Serialización (WritePacket): RankType, MyRank, ListingDetails.Count, Listings.Count, Listings[i], Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadByte, ReadInt32, ReadInt32, [Nested new(..., reader)]

### [251] Opendoor  clase Opendoor
- Dirección: Server
- Serialización (WritePacket): DoorIndex, Close
- Deserialización (ReadPacket): ReadByte, ReadBoolean

### [252] GetRentedItems  clase GetRentedItems
- Dirección: Server
- Serialización (WritePacket): RentedItems.Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadInt32, [Nested new(..., reader)]

### [253] ItemRentalRequest  clase ItemRentalRequest
- Dirección: Server
- Serialización (WritePacket): Name, Renting
- Deserialización (ReadPacket): ReadString, ReadBoolean

### [254] ItemRentalFee  clase ItemRentalFee
- Dirección: Server
- Serialización (WritePacket): Amount
- Deserialización (ReadPacket): ReadUInt32

### [255] ItemRentalPeriod  clase ItemRentalPeriod
- Dirección: Server
- Serialización (WritePacket): Days
- Deserialización (ReadPacket): ReadUInt32

### [256] DepositRentalItem  clase DepositRentalItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [257] RetrieveRentalItem  clase RetrieveRentalItem
- Dirección: Server
- Serialización (WritePacket): From, To, Success
- Deserialización (ReadPacket): ReadInt32, ReadInt32, ReadBoolean

### [258] UpdateRentalItem  clase UpdateRentalItem
- Dirección: Server
- Serialización (WritePacket): LoanItem != null, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadBoolean, [Nested new(..., reader)]

### [259] CancelItemRental  clase CancelItemRental
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [260] ItemRentalLock  clase ItemRentalLock
- Dirección: Server
- Serialización (WritePacket): Success, GoldLocked, ItemLocked
- Deserialización (ReadPacket): ReadBoolean, ReadBoolean, ReadBoolean

### [261] ItemRentalPartnerLock  clase ItemRentalPartnerLock
- Dirección: Server
- Serialización (WritePacket): GoldLocked, ItemLocked
- Deserialización (ReadPacket): ReadBoolean, ReadBoolean

### [262] CanConfirmItemRental  clase CanConfirmItemRental
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [263] ConfirmItemRental  clase ConfirmItemRental
- Dirección: Server
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [264] NewRecipeInfo  clase NewRecipeInfo
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [265] OpenBrowser  clase OpenBrowser
- Dirección: Server
- Serialización (WritePacket): Url
- Deserialización (ReadPacket): ReadString

### [266] PlaySound  clase PlaySound
- Dirección: Server
- Serialización (WritePacket): Sound
- Deserialización (ReadPacket): ReadInt32

### [267] SetTimer  clase SetTimer
- Dirección: Server
- Serialización (WritePacket): Key, Type, Seconds
- Deserialización (ReadPacket): ReadString, ReadByte, ReadInt32

### [268] ExpireTimer  clase ExpireTimer
- Dirección: Server
- Serialización (WritePacket): Key
- Deserialización (ReadPacket): ReadString

### [269] UpdateNotice  clase UpdateNotice
- Dirección: Server
- Serialización (WritePacket): [Nested Save(writer)]
- Deserialización (ReadPacket): [Nested new(..., reader)]

### [270] Roll  clase Roll
- Dirección: Server
- Serialización (WritePacket): Type, Page, Result, AutoRoll
- Deserialización (ReadPacket): ReadInt32, ReadString, ReadInt32, ReadBoolean

### [271] SetCompass  clase SetCompass
- Dirección: Server
- Serialización (WritePacket): Location.X, Location.Y
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [272] GroupMembersMap  clase GroupMembersMap
- Dirección: Server
- Serialización (WritePacket): PlayerName, PlayerMap
- Deserialización (ReadPacket): ReadString, ReadString

### [273] SendMemberLocation  clase SendMemberLocation
- Dirección: Server
- Serialización (WritePacket): MemberName, MemberLocation.X, MemberLocation.Y
- Deserialización (ReadPacket): ReadString


## Client  Server

### [0] ClientVersion  clase ClientVersion
- Dirección: Client
- Serialización (WritePacket): VersionHash.Length, VersionHash
- Deserialización (ReadPacket): (no detectada)

### [1] Disconnect  clase Disconnect
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [2] KeepAlive  clase KeepAlive
- Dirección: Client
- Serialización (WritePacket): Time
- Deserialización (ReadPacket): ReadInt64

### [3] NewAccount  clase NewAccount
- Dirección: Client
- Serialización (WritePacket): AccountID, Password, BirthDate.ToBinary(), UserName, SecretQuestion, SecretAnswer, EMailAddress
- Deserialización (ReadPacket): ReadString, ReadString, ReadString, ReadString, ReadString, ReadString

### [4] ChangePassword  clase ChangePassword
- Dirección: Client
- Serialización (WritePacket): AccountID, CurrentPassword, NewPassword
- Deserialización (ReadPacket): ReadString, ReadString, ReadString

### [5] Login  clase Login
- Dirección: Client
- Serialización (WritePacket): AccountID, Password
- Deserialización (ReadPacket): ReadString, ReadString

### [6] NewCharacter  clase NewCharacter
- Dirección: Client
- Serialización (WritePacket): Name, (byte)Gender, (byte)Class
- Deserialización (ReadPacket): ReadString

### [7] DeleteCharacter  clase DeleteCharacter
- Dirección: Client
- Serialización (WritePacket): CharacterIndex
- Deserialización (ReadPacket): ReadInt32

### [8] StartGame  clase StartGame
- Dirección: Client
- Serialización (WritePacket): CharacterIndex
- Deserialización (ReadPacket): ReadInt32

### [9] LogOut  clase LogOut
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [10] Turn  clase Turn
- Dirección: Client
- Serialización (WritePacket): (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [11] Walk  clase Walk
- Dirección: Client
- Serialización (WritePacket): (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [12] Run  clase Run
- Dirección: Client
- Serialización (WritePacket): (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [13] Chat  clase Chat
- Dirección: Client
- Serialización (WritePacket): Message, LinkedItems.Count, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadString, ReadInt32, [Nested new(..., reader)]

### [14] MoveItem  clase MoveItem
- Dirección: Client
- Serialización (WritePacket): (byte)Grid, From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [15] StoreItem  clase StoreItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [16] TakeBackItem  clase TakeBackItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [17] MergeItem  clase MergeItem
- Dirección: Client
- Serialización (WritePacket): (byte)GridFrom, (byte)GridTo, IDFrom, IDTo
- Deserialización (ReadPacket): ReadUInt64, ReadUInt64

### [18] EquipItem  clase EquipItem
- Dirección: Client
- Serialización (WritePacket): (byte)Grid, UniqueID, To
- Deserialización (ReadPacket): ReadUInt64, ReadInt32

### [19] RemoveItem  clase RemoveItem
- Dirección: Client
- Serialización (WritePacket): (byte)Grid, UniqueID, To
- Deserialización (ReadPacket): ReadUInt64, ReadInt32

### [20] RemoveSlotItem  clase RemoveSlotItem
- Dirección: Client
- Serialización (WritePacket): (byte)Grid, (byte)GridTo, UniqueID, To, FromUniqueID
- Deserialización (ReadPacket): ReadUInt64, ReadInt32, ReadUInt64

### [21] SplitItem  clase SplitItem
- Dirección: Client
- Serialización (WritePacket): (byte)Grid, UniqueID, Count
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16

### [22] UseItem  clase UseItem
- Dirección: Client
- Serialización (WritePacket): UniqueID, (byte)Grid
- Deserialización (ReadPacket): ReadUInt64

### [23] DropItem  clase DropItem
- Dirección: Client
- Serialización (WritePacket): UniqueID, Count, HeroInventory
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16, ReadBoolean

### [24] DepositRefineItem  clase DepositRefineItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [25] RetrieveRefineItem  clase RetrieveRefineItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [26] RefineCancel  clase RefineCancel
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [27] RefineItem  clase RefineItem
- Dirección: Client
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [28] CheckRefine  clase CheckRefine
- Dirección: Client
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [29] ReplaceWedRing  clase ReplaceWedRing
- Dirección: Client
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [30] DepositTradeItem  clase DepositTradeItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [31] RetrieveTradeItem  clase RetrieveTradeItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [32] TakeBackHeroItem  clase TakeBackHeroItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [33] TransferHeroItem  clase TransferHeroItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [34] DropGold  clase DropGold
- Dirección: Client
- Serialización (WritePacket): Amount
- Deserialización (ReadPacket): ReadUInt32

### [35] PickUp  clase PickUp
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [36] RequestMapInfo  clase RequestMapInfo
- Dirección: Client
- Serialización (WritePacket): MapIndex
- Deserialización (ReadPacket): ReadInt32

### [37] TeleportToNPC  clase TeleportToNPC
- Dirección: Client
- Serialización (WritePacket): ObjectID
- Deserialización (ReadPacket): ReadUInt32

### [38] SearchMap  clase SearchMap
- Dirección: Client
- Serialización (WritePacket): Text
- Deserialización (ReadPacket): ReadString

### [39] Inspect  clase Inspect
- Dirección: Client
- Serialización (WritePacket): ObjectID, Ranking, Hero
- Deserialización (ReadPacket): ReadUInt32, ReadBoolean, ReadBoolean

### [40] Observe  clase Observe
- Dirección: Client
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [41] ChangeAMode  clase ChangeAMode
- Dirección: Client
- Serialización (WritePacket): (byte)Mode
- Deserialización (ReadPacket): (no detectada)

### [42] ChangePMode  clase ChangePMode
- Dirección: Client
- Serialización (WritePacket): (byte)Mode
- Deserialización (ReadPacket): (no detectada)

### [43] ChangeTrade  clase ChangeTrade
- Dirección: Client
- Serialización (WritePacket): AllowTrade
- Deserialización (ReadPacket): ReadBoolean

### [44] Attack  clase Attack
- Dirección: Client
- Serialización (WritePacket): (byte)Direction, (byte)Spell
- Deserialización (ReadPacket): (no detectada)

### [45] RangeAttack  clase RangeAttack
- Dirección: Client
- Serialización (WritePacket): (byte)Direction, Location.X, Location.Y, TargetID, TargetLocation.X, TargetLocation.Y
- Deserialización (ReadPacket): ReadUInt32

### [46] Harvest  clase Harvest
- Dirección: Client
- Serialización (WritePacket): (byte)Direction
- Deserialización (ReadPacket): (no detectada)

### [47] CallNPC  clase CallNPC
- Dirección: Client
- Serialización (WritePacket): ObjectID, Key
- Deserialización (ReadPacket): ReadUInt32, ReadString

### [48] BuyItem  clase BuyItem
- Dirección: Client
- Serialización (WritePacket): ItemIndex, Count, (byte)Type
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16

### [49] SellItem  clase SellItem
- Dirección: Client
- Serialización (WritePacket): UniqueID, Count
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16

### [50] CraftItem  clase CraftItem
- Dirección: Client
- Serialización (WritePacket): UniqueID, Count, Slots.Length, Slots[i]
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16, ReadInt32

### [51] RepairItem  clase RepairItem
- Dirección: Client
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [52] BuyItemBack  clase BuyItemBack
- Dirección: Client
- Serialización (WritePacket): UniqueID, Count
- Deserialización (ReadPacket): ReadUInt64, ReadUInt16

### [53] SRepairItem  clase SRepairItem
- Dirección: Client
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [54] MagicKey  clase MagicKey
- Dirección: Client
- Serialización (WritePacket): (byte) Spell, Key, OldKey
- Deserialización (ReadPacket): ReadByte, ReadByte

### [55] Magic  clase Magic
- Dirección: Client
- Serialización (WritePacket): ObjectID, (byte) Spell, (byte)Direction, TargetID, Location.X, Location.Y, SpellTargetLock
- Deserialización (ReadPacket): ReadUInt32, ReadUInt32, ReadBoolean

### [56] SwitchGroup  clase SwitchGroup
- Dirección: Client
- Serialización (WritePacket): AllowGroup
- Deserialización (ReadPacket): ReadBoolean

### [57] AddMember  clase AddMember
- Dirección: Client
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [58] DellMember  clase DelMember
- Dirección: Client
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [59] GroupInvite  clase GroupInvite
- Dirección: Client
- Serialización (WritePacket): AcceptInvite
- Deserialización (ReadPacket): ReadBoolean

### [60] NewHero  clase NewHero
- Dirección: Client
- Serialización (WritePacket): Name, (byte)Gender, (byte)Class
- Deserialización (ReadPacket): ReadString

### [61] SetAutoPotValue  clase SetAutoPotValue
- Dirección: Client
- Serialización (WritePacket): (byte)Stat, Value
- Deserialización (ReadPacket): ReadUInt32

### [62] SetAutoPotItem  clase SetAutoPotItem
- Dirección: Client
- Serialización (WritePacket): (byte)Grid, ItemIndex
- Deserialización (ReadPacket): ReadInt32

### [63] SetHeroBehaviour  clase SetHeroBehaviour
- Dirección: Client
- Serialización (WritePacket): (byte)Behaviour
- Deserialización (ReadPacket): (no detectada)

### [64] ChangeHero  clase ChangeHero
- Dirección: Client
- Serialización (WritePacket): ListIndex
- Deserialización (ReadPacket): ReadInt32

### [65] TownRevive  clase TownRevive
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [66] SpellToggle  clase SpellToggle
- Dirección: Client
- Serialización (WritePacket): (byte)Spell, (sbyte)canUse
- Deserialización (ReadPacket): (no detectada)

### [67] ConsignItem  clase ConsignItem
- Dirección: Client
- Serialización (WritePacket): UniqueID, Price, (byte)Type
- Deserialización (ReadPacket): ReadUInt64, ReadUInt32

### [68] MarketSearch  clase MarketSearch
- Dirección: Client
- Serialización (WritePacket): Match, (Byte)Type, Usermode, MinShape, MaxShape, (byte)MarketType
- Deserialización (ReadPacket): ReadString, ReadBoolean, ReadInt16, ReadInt16

### [69] MarketRefresh  clase MarketRefresh
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [70] MarketPage  clase MarketPage
- Dirección: Client
- Serialización (WritePacket): Page
- Deserialización (ReadPacket): ReadInt32

### [71] MarketBuy  clase MarketBuy
- Dirección: Client
- Serialización (WritePacket): AuctionID, BidPrice
- Deserialización (ReadPacket): ReadUInt64, ReadUInt32

### [72] MarketGetBack  clase MarketGetBack
- Dirección: Client
- Serialización (WritePacket): AuctionID
- Deserialización (ReadPacket): ReadUInt64

### [73] MarketSellNow  clase MarketSellNow
- Dirección: Client
- Serialización (WritePacket): AuctionID
- Deserialización (ReadPacket): ReadUInt64

### [74] RequestUserName  clase RequestUserName
- Dirección: Client
- Serialización (WritePacket): UserID
- Deserialización (ReadPacket): ReadUInt32

### [75] RequestChatItem  clase RequestChatItem
- Dirección: Client
- Serialización (WritePacket): ChatItemID
- Deserialización (ReadPacket): ReadUInt64

### [76] EditGuildMember  clase EditGuildMember
- Dirección: Client
- Serialización (WritePacket): ChangeType, RankIndex, Name, RankName
- Deserialización (ReadPacket): ReadByte, ReadByte, ReadString, ReadString

### [77] EditGuildNotice  clase EditGuildNotice
- Dirección: Client
- Serialización (WritePacket): notice.Count, notice[i]
- Deserialización (ReadPacket): ReadInt32

### [78] GuildInvite  clase GuildInvite
- Dirección: Client
- Serialización (WritePacket): AcceptInvite
- Deserialización (ReadPacket): ReadBoolean

### [79] GuildNameReturn  clase GuildNameReturn
- Dirección: Client
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [80] RequestGuildInfo  clase RequestGuildInfo
- Dirección: Client
- Serialización (WritePacket): Type
- Deserialización (ReadPacket): ReadByte

### [81] GuildStorageGoldChange  clase GuildStorageGoldChange
- Dirección: Client
- Serialización (WritePacket): Type, Amount
- Deserialización (ReadPacket): ReadByte, ReadUInt32

### [82] GuildStorageItemChange  clase GuildStorageItemChange
- Dirección: Client
- Serialización (WritePacket): Type, From, To
- Deserialización (ReadPacket): ReadByte, ReadInt32, ReadInt32

### [83] GuildWarReturn  clase GuildWarReturn
- Dirección: Client
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [84] MarriageRequest  clase MarriageRequest
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [85] MarriageReply  clase MarriageReply
- Dirección: Client
- Serialización (WritePacket): AcceptInvite
- Deserialización (ReadPacket): ReadBoolean

### [86] ChangeMarriage  clase ChangeMarriage
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [87] DivorceRequest  clase DivorceRequest
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [88] DivorceReply  clase DivorceReply
- Dirección: Client
- Serialización (WritePacket): AcceptInvite
- Deserialización (ReadPacket): ReadBoolean

### [89] AddMentor  clase AddMentor
- Dirección: Client
- Serialización (WritePacket): Name
- Deserialización (ReadPacket): ReadString

### [90] MentorReply  clase MentorReply
- Dirección: Client
- Serialización (WritePacket): AcceptInvite
- Deserialización (ReadPacket): ReadBoolean

### [91] AllowMentor  clase AllowMentor
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [92] CancelMentor  clase CancelMentor
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [93] TradeRequest  clase TradeRequest
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [94] TradeReply  clase TradeReply
- Dirección: Client
- Serialización (WritePacket): AcceptInvite
- Deserialización (ReadPacket): ReadBoolean

### [95] TradeGold  clase TradeGold
- Dirección: Client
- Serialización (WritePacket): Amount
- Deserialización (ReadPacket): ReadUInt32

### [96] TradeConfirm  clase TradeConfirm
- Dirección: Client
- Serialización (WritePacket): Locked
- Deserialización (ReadPacket): ReadBoolean

### [97] TradeCancel  clase TradeCancel
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [98] EquipSlotItem  clase EquipSlotItem
- Dirección: Client
- Serialización (WritePacket): (byte)Grid, UniqueID, To, (byte)GridTo, ToUniqueID
- Deserialización (ReadPacket): ReadUInt64, ReadInt32, ReadUInt64

### [99] FishingCast  clase FishingCast
- Dirección: Client
- Serialización (WritePacket): CastOut
- Deserialización (ReadPacket): ReadBoolean

### [100] FishingChangeAutocast  clase FishingChangeAutocast
- Dirección: Client
- Serialización (WritePacket): AutoCast
- Deserialización (ReadPacket): ReadBoolean

### [101] AcceptQuest  clase AcceptQuest
- Dirección: Client
- Serialización (WritePacket): NPCIndex, QuestIndex
- Deserialización (ReadPacket): ReadUInt32, ReadInt32

### [102] FinishQuest  clase FinishQuest
- Dirección: Client
- Serialización (WritePacket): QuestIndex, SelectedItemIndex
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [103] AbandonQuest  clase AbandonQuest
- Dirección: Client
- Serialización (WritePacket): QuestIndex
- Deserialización (ReadPacket): ReadInt32

### [104] ShareQuest  clase ShareQuest
- Dirección: Client
- Serialización (WritePacket): QuestIndex
- Deserialización (ReadPacket): ReadInt32

### [105] AcceptReincarnation  clase AcceptReincarnation
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [106] CancelReincarnation  clase CancelReincarnation
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [107] CombineItem  clase CombineItem
- Dirección: Client
- Serialización (WritePacket): (byte)Grid, IDFrom, IDTo
- Deserialización (ReadPacket): ReadUInt64, ReadUInt64

### [108] AwakeningNeedMaterials  clase AwakeningNeedMaterials
- Dirección: Client
- Serialización (WritePacket): UniqueID, (byte)Type
- Deserialización (ReadPacket): ReadUInt64

### [109] AwakeningLockedItem  clase AwakeningLockedItem
- Dirección: Client
- Serialización (WritePacket): UniqueID, Locked
- Deserialización (ReadPacket): ReadUInt64, ReadBoolean

### [110] Awakening  clase Awakening
- Dirección: Client
- Serialización (WritePacket): UniqueID, (byte)Type, PositionIdx
- Deserialización (ReadPacket): ReadUInt64, ReadUInt32

### [111] DisassembleItem  clase DisassembleItem
- Dirección: Client
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [112] DowngradeAwakening  clase DowngradeAwakening
- Dirección: Client
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [113] ResetAddedItem  clase ResetAddedItem
- Dirección: Client
- Serialización (WritePacket): UniqueID
- Deserialización (ReadPacket): ReadUInt64

### [114] SendMail  clase SendMail
- Dirección: Client
- Serialización (WritePacket): Name, Message, Gold, ItemsIdx[i]
- Deserialización (ReadPacket): ReadString, ReadString, ReadUInt32, ReadUInt64

### [115] ReadMail  clase ReadMail
- Dirección: Client
- Serialización (WritePacket): MailID
- Deserialización (ReadPacket): ReadUInt64

### [116] CollectParcel  clase CollectParcel
- Dirección: Client
- Serialización (WritePacket): MailID
- Deserialización (ReadPacket): ReadUInt64

### [117] DeleteMail  clase DeleteMail
- Dirección: Client
- Serialización (WritePacket): MailID
- Deserialización (ReadPacket): ReadUInt64

### [118] LockMail  clase LockMail
- Dirección: Client
- Serialización (WritePacket): MailID, Lock
- Deserialización (ReadPacket): ReadUInt64, ReadBoolean

### [119] MailLockedItem  clase MailLockedItem
- Dirección: Client
- Serialización (WritePacket): UniqueID, Locked
- Deserialización (ReadPacket): ReadUInt64, ReadBoolean

### [120] MailCost  clase MailCost
- Dirección: Client
- Serialización (WritePacket): Gold, ItemsIdx[i]
- Deserialización (ReadPacket): ReadUInt32, ReadUInt64

### [121] UpdateIntelligentCreature  clase UpdateIntelligentCreature
- Dirección: Client
- Serialización (WritePacket): SummonMe, UnSummonMe, ReleaseMe, [Nested Save(writer)]
- Deserialización (ReadPacket): ReadBoolean, ReadBoolean, ReadBoolean, [Nested new(..., reader)]

### [122] IntelligentCreaturePickup  clase IntelligentCreaturePickup
- Dirección: Client
- Serialización (WritePacket): MouseMode, Location.X, Location.Y
- Deserialización (ReadPacket): ReadBoolean, ReadInt32, ReadInt32

### [123] RequestIntelligentCreatureUpdates  clase RequestIntelligentCreatureUpdates
- Dirección: Client
- Serialización (WritePacket): Update
- Deserialización (ReadPacket): ReadBoolean

### [124] AddFriend  clase AddFriend
- Dirección: Client
- Serialización (WritePacket): Name, Blocked
- Deserialización (ReadPacket): ReadString, ReadBoolean

### [125] RemoveFriend  clase RemoveFriend
- Dirección: Client
- Serialización (WritePacket): CharacterIndex
- Deserialización (ReadPacket): ReadInt32

### [126] RefreshFriends  clase RefreshFriends
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [127] AddMemo  clase AddMemo
- Dirección: Client
- Serialización (WritePacket): CharacterIndex, Memo
- Deserialización (ReadPacket): ReadInt32, ReadString

### [128] GuildBuffUpdate  clase GuildBuffUpdate
- Dirección: Client
- Serialización (WritePacket): Action, Id
- Deserialización (ReadPacket): ReadByte, ReadInt32

### [129] NPCConfirmInput  clase NPCConfirmInput
- Dirección: Client
- Serialización (WritePacket): NPCID, PageName, Value
- Deserialización (ReadPacket): ReadUInt32, ReadString, ReadString

### [130] GameshopBuy  clase GameshopBuy
- Dirección: Client
- Serialización (WritePacket): GIndex, Quantity, PType
- Deserialización (ReadPacket): ReadInt32, ReadByte, ReadInt32

### [131] ReportIssue  clase ReportIssue
- Dirección: Client
- Serialización (WritePacket): Image.Length, Image, ImageSize, ImageChunk
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [132] GetRanking  clase GetRanking
- Dirección: Client
- Serialización (WritePacket): RankType, RankIndex, OnlineOnly
- Deserialización (ReadPacket): ReadByte, ReadInt32, ReadBoolean

### [133] Opendoor  clase Opendoor
- Dirección: Client
- Serialización (WritePacket): DoorIndex
- Deserialización (ReadPacket): ReadByte

### [134] GetRentedItems  clase GetRentedItems
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [135] ItemRentalRequest  clase ItemRentalRequest
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [136] ItemRentalFee  clase ItemRentalFee
- Dirección: Client
- Serialización (WritePacket): Amount
- Deserialización (ReadPacket): ReadUInt32

### [137] ItemRentalPeriod  clase ItemRentalPeriod
- Dirección: Client
- Serialización (WritePacket): Days
- Deserialización (ReadPacket): ReadUInt32

### [138] DepositRentalItem  clase DepositRentalItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [139] RetrieveRentalItem  clase RetrieveRentalItem
- Dirección: Client
- Serialización (WritePacket): From, To
- Deserialización (ReadPacket): ReadInt32, ReadInt32

### [140] CancelItemRental  clase CancelItemRental
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [141] ItemRentalLockFee  clase ItemRentalLockFee
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [142] ItemRentalLockItem  clase ItemRentalLockItem
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

### [143] ConfirmItemRental  clase ConfirmItemRental
- Dirección: Client
- Serialización (WritePacket): (no detectada)
- Deserialización (ReadPacket): (no detectada)

