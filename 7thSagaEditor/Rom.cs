using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Diagnostics;



namespace SevenSagaEditor.RomInfo
{
    public enum ByteOrder : int
    {
        HighByteFirst = 0,
        LowByteFirst = 1
    }

    class Rom
    {


        public static string endOfNameByte = "F7";
        public static int maxCharacters = 7;
        public static int maxItems = 100;
        public static int maxWeapons = 51;
        public static int maxArmors = 105;
        public static int maxLearn = 16;
        public static int maxSpells = 61;
        public static int maxMonsters = 98;
        public static int maxMonsterSpells = 8;
        public static int maxElements = 7;
        public static int maxDropSets = 16;
        public static int maxItemsPerDropSet = 16;
        public static int maxExperienceTable = 81;
        public int[] charHPBase = new int[maxCharacters];
        public int[] charHPGrowth = new int[maxCharacters];
        public int[] charMPBase = new int[maxCharacters];
        public int[] charMPGrowth = new int[maxCharacters];
        public int[] charPowerBase = new int[maxCharacters];
        public int[] charPowerGrowth = new int[maxCharacters];
        public int[] charGuardBase = new int[maxCharacters];
        public int[] charGuardGrowth = new int[maxCharacters];
        public int[] charMagicBase = new int[maxCharacters];
        public int[] charMagicGrowth = new int[maxCharacters];
        public int[] charSpeedBase = new int[maxCharacters];
        public int[] charSpeedGrowth = new int[maxCharacters];
        public int[] charStartWeapon = new int[maxCharacters];      // when saving this value, remember to add "100" (weapon list after item list, whose size is 100)
        public int[] charStartArmor = new int[maxCharacters];
        public int[] charStartAccessory = new int[maxCharacters];
        public int[] charStartExp = new int[maxCharacters];
        public string[] charOffset = new string[maxCharacters];

        public int[,] charLearnSpells = new int[maxCharacters, maxLearn];
        public int[,] charLearnLevels = new int[maxCharacters, maxLearn];

        public string[] spellName = new string[maxSpells];
        public string[] spellNameOffset = new string[maxSpells];
        public int[] spellPower = new int[maxSpells];
        public string[] spellTarget = new string[maxSpells];
        public int[] spellCost = new int[maxSpells];
        public string[] spellDomain = new string[maxSpells];
        public string[] spellElement = new string[maxSpells];
        public string[] spellUnknown1 = new string[maxSpells];
        public string[] spellUnknown2 = new string[maxSpells];


        public string[] itemName = new string[maxItems];
        public string[] itemNameOffset = new string[maxItems];
        public string[] itemTarget = new string[maxItems];
        public string[] itemSingleUse = new string[maxItems];
        public int[] itemCost = new int[maxItems];
        public string[] itemUsers = new string[maxItems];
        public string[] itemSellRatio = new string[maxItems];


        public string[] weaponName = new string[maxWeapons];
        public string[] weaponNameOffset = new string[maxWeapons];
        public int[] weaponPower = new int[maxWeapons];
        public int[] weaponCost = new int[maxWeapons];
        public string[] weaponUsers = new string[maxWeapons];
        public string[] weaponUnknown = new string[maxWeapons];

        public string[] armorName = new string[maxArmors];
        public string[] armorNameOffset = new string[maxArmors];
        public int[] armorGuard = new int[maxArmors];
        public int[] armorCost = new int[maxArmors];
        public string[] armorUsers = new string[maxArmors];
        public int[] armorThunderRes = new int[maxArmors];
        public int[] armorUnknownRes1 = new int[maxArmors];
        public int[] armorUnknownRes2 = new int[maxArmors];
        public int[] armorFireRes = new int[maxArmors];
        public int[] armorIceRes = new int[maxArmors];
        public int[] armorVacuumRes = new int[maxArmors];
        public int[] armorMagicRes = new int[maxArmors];
        public string[] armorUnknown = new string[maxArmors];
        public string[] armorUnknownBin = new string[maxArmors];

        public string[] monsterName = new string[maxMonsters];
        public string[] monsterNameOffset = new string[maxMonsters];
        public string[] monsterUnknown1 = new string[maxMonsters];
        public int[] monsterMaxHP = new int[maxMonsters];
        public int[] monsterMaxMP = new int[maxMonsters];
        public int[] monsterPower = new int[maxMonsters];
        public int[] monsterGuard = new int[maxMonsters];
        public int[] monsterMagic = new int[maxMonsters];
        public int[] monsterSpeed = new int[maxMonsters];
        public int[,] monsterSpells = new int[maxMonsters, maxLearn];
        public int[,] monsterSpellsChance = new int[maxMonsters, maxLearn];
        public int[,] monsterResistances = new int[maxMonsters, maxElements];
        public int[] monsterGold = new int[maxMonsters];
        public int[] monsterDropSet = new int[maxMonsters];
        public string[] monsterUnknown2 = new string[maxMonsters];
        public string[] monsterRunFlag = new string[maxMonsters];

        public int[,] dropSets = new int[maxDropSets, maxItemsPerDropSet];

        public int[] experienceTable = new int[maxExperienceTable];

        private Dictionary<string, string> characterMap = new Dictionary<string, string>();
        private byte[] Data;
        private bool _header;

        public bool Header
        {
            get
            {
                return _header;
            }
        }

        public Rom(string path)
        {
            
            this.Data = File.ReadAllBytes(path);

            int remainder = this.Data.Length & 0x7FFF;
            this._header = remainder == 0x200;
            setupCharacterMap();
            loadCharacterData();
            loadSpellData();
            loadItemData();
            loadWeaponData();
            loadArmorData();
            loadMonsterData();
            loadDropSetData();
            loadExperienceTable();
        }


        /*****************************************************
         * 
         * SubData
         * 
        *****************************************************/

        public byte[] SubData(string offset, int length)
        {

            int index = int.Parse(offset, System.Globalization.NumberStyles.HexNumber);
            //index -= 1024;
            //index += 512;

            byte[] result = new byte[length];
            Array.Copy(Data, index, result, 0, length);
            return result;
        }


        /*****************************************************
         * 
         * readDataInt
         * 
        *****************************************************/

        public int readDataInt(string offset, int length, bool lEndian)
        {
            byte[] buffer = new byte[length];

            buffer = SubData(offset, length);
            System.Text.StringBuilder hexString = new System.Text.StringBuilder(length * 2);

            for (int index = 0; index < length; index++)
            {
                byte byteValue = buffer[index];
                string hexValue = byteValue.ToString("X");
                if (hexValue.Length < 2) { hexValue = "0" + hexValue; }
                if (!lEndian)
                {
                    hexString.Append(hexValue);
                }
                else
                {
                    hexString.Insert(0, hexValue);
                }
            }
            return (int.Parse(hexString.ToString(), System.Globalization.NumberStyles.HexNumber));
        }


        /*****************************************************
         * 
         * readDataByte
         * 
        *****************************************************/
        public string readDataByte(string offset, int length, bool lEndian)
        {
            byte[] buffer = new byte[length];

            buffer = SubData(offset, length);
            System.Text.StringBuilder hexString = new System.Text.StringBuilder(length * 2);

            for (int index = 0; index < length; index++)
            {
                byte byteValue = buffer[index];
                string hexValue = byteValue.ToString("X");
                if (hexValue.Length < 2) { hexValue = "0" + hexValue; }
                if (!lEndian)
                {
                    hexString.Append(hexValue);
                }
                else
                {
                    hexString.Insert(0, hexValue);
                }
            }
            return (hexString.ToString());
        }


        /*****************************************************
         * 
         * writeDataByte
         * 
        *****************************************************/
        public void writeDataByte(string value, string offset, int length, bool lEndian)
        {
            length *= 2;

            String hexString = value;

            int index = int.Parse(offset, System.Globalization.NumberStyles.HexNumber);
            //index -= 1024;
            //index += 512;

            if (!lEndian)
            {
                for (int i = 0; i < length; i += 2)
                {
                    String substring = hexString.Substring(i, 2);
                    byte b = Convert.ToByte(substring, 16);
                    Data[index + (i / 2)] = b;
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder("0000");
                for (int i = 0; i < length; i += 2)
                {
                    String substring = hexString.Substring(i, 2);
                    byte b = Convert.ToByte(substring, 16);
                    Data[index + ((length - i - 2) / 2)] = b;
                    sb[((length - i - 2) / 2)] = substring[0];
                    sb[((length - i - 2) / 2) + 1] = substring[1];

                }
            }
        }


        /*****************************************************
         * 
         * writeDataInt
         * 
        *****************************************************/
        public void writeDataInt(int value, string offset, int length, bool lEndian)
        {
            length *= 2;

            String hexString = value.ToString("X" + (length).ToString());

            int index = int.Parse(offset, System.Globalization.NumberStyles.HexNumber);
            //index -= 1024;
            //index += 512;

            if (!lEndian)
            {
                for (int i = 0; i < length; i += 2)
                {
                    String substring = hexString.Substring(i, 2);
                    byte b = Convert.ToByte(substring, 16);
                    Data[index + (i / 2)] = b;
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder("0000");
                for (int i = 0; i < length; i += 2)
                {
                    String substring = hexString.Substring(i, 2);
                    byte b = Convert.ToByte(substring, 16);
                    Data[index + ((length - i - 2) / 2)] = b;
                    sb[((length - i - 2) / 2)] = substring[0];
                    sb[((length - i - 2) / 2) + 1] = substring[1];

                }
            }
        }



        /*****************************************************
         * 
         * SaveData
         * 
        *****************************************************/

        public void SaveData(string ROMname)
        {
            saveCharacterData();
            saveSpellData();
            saveMonsterData();
            saveWeaponData();
            saveArmorData();
            saveDropSetData();
            saveExperienceTable();
            File.WriteAllBytes(ROMname, Data);
        }



        /*****************************************************
         * 
         * hexAdd
         * 
        *****************************************************/
        public string hexAdd(string value1, string value2)
        {

            string r;
            int v1 = int.Parse(value1, System.Globalization.NumberStyles.HexNumber);
            int v2 = int.Parse(value2, System.Globalization.NumberStyles.HexNumber);
            r = (v1 + v2).ToString("X");
            return r;
        }



        /*****************************************************
         * 
         * hexSub
         * 
        *****************************************************/

        public string hexSub(string value1, string value2)
        {
            string r;
            int v1 = int.Parse(value1, System.Globalization.NumberStyles.HexNumber);
            int v2 = int.Parse(value2, System.Globalization.NumberStyles.HexNumber);
            r = (v1 - v2).ToString("X");
            return r;
        }



        /*****************************************************
         * 
         * hexMult
         * 
        *****************************************************/

        public string hexMult(string value, int num)
        {
            string r;
            int i, total = 0;
            for (i = 0; i < num; i++)
            {
                total += int.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
            r = total.ToString("X");
            return r;
        }



        /*****************************************************
         * 
         * readDataHex
         * 
        *****************************************************/

        public string readDataHex(string offset)
        {
            byte[] buffer = new byte[1];

            buffer = SubData(offset, 1);

            byte byteValue = buffer[0];
            return (byteValue.ToString("X2"));
        }



        /*****************************************************
         * 
         * loadCharacterData
         * 
        *****************************************************/

        private void loadCharacterData()
        {
            string startOffset = "00623F";
            string offset;
            int dataSize;
            

            for (var i = 0; i < maxCharacters; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("12", i));    // 12 is 18 in hex
                charOffset[i] = offset;

                dataSize = 2;
                charHPBase[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                charMPBase[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                charPowerBase[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                charGuardBase[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                charMagicBase[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                charSpeedBase[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());


                dataSize = 1;
                charHPGrowth[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                charMPGrowth[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                charPowerGrowth[i] = readDataInt(offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                charGuardGrowth[i] = readDataInt(offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                charMagicGrowth[i] = readDataInt(offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                charSpeedGrowth[i] = readDataInt(offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                
                charStartWeapon[i] = readDataInt(offset, dataSize, false)-100;
                offset = hexAdd(offset, dataSize.ToString());
                charStartArmor[i] = readDataInt(offset, dataSize, false)-151;
                offset = hexAdd(offset, dataSize.ToString());
                charStartAccessory[i] = readDataInt(offset, dataSize, false)-151;
                offset = hexAdd(offset, dataSize.ToString());
                charStartExp[i] = readDataInt(offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());

                loadLearnData(i);
            }
        }



        /*****************************************************
         * 
         * loadLearnData
         * 
        *****************************************************/

        private void loadLearnData(int characterIndex)
        {
            string startOffset = "0062BD";
            startOffset = hexAdd(startOffset, this.hexMult("20", characterIndex)); // 20 is 32 in hex
            string offset;
            int dataSize = 1;

            for (var i = 0; i < maxLearn; i += 1)
            {
                offset = hexAdd(startOffset, this.hexMult("1", i));
                charLearnSpells[characterIndex, i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, "10");
                charLearnLevels[characterIndex, i] = readDataInt(offset, dataSize, true);
            }

        }



        /*****************************************************
         * 
         * loadItemData
         * 
        *****************************************************/

        private void loadItemData()
        {
            string startOffset = "006C94";
            string offset;
            int dataSize;
            for (var i = 0; i < maxItems; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("09", i));
                //charOffset[i] = offset;

                //this.charNames[i] = this.romData.readDataString(offset, 1);
                dataSize = 1;
                itemTarget[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                itemSingleUse[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 2;
                itemCost[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 1;
                itemUsers[i] = readDataByte(offset, dataSize, true);
                itemUsers[i] = hex2bin(itemUsers[i]);
                offset = hexAdd(offset, dataSize.ToString());

                itemSellRatio[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 3;
                itemNameOffset[i] = readDataByte(offset, dataSize, true);
                itemName[i] = buildString(itemNameOffset[i], 10);

            }
        }


        /*****************************************************
         * 
         * loadWeaponData
         * 
        *****************************************************/

        private void loadWeaponData()
        {
            string startOffset = "00639D";
            string offset;
            int dataSize;
            for (var i = 0; i < maxWeapons; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("0A", i));    // 0A is 10 in hex
                //charOffset[i] = offset;
                //this.charNames[i] = this.romData.readDataString(offset, 1);
                dataSize = 2;
                weaponPower[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                weaponCost[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 1;
                weaponUsers[i] = readDataByte(offset, dataSize, true);
                weaponUsers[i] = hex2bin(weaponUsers[i]);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 3;
                weaponNameOffset[i] = readDataByte(offset, dataSize, true);
                weaponName[i] = buildString(weaponNameOffset[i], 10);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 2;
                weaponUnknown[i] = readDataByte(offset, dataSize, true);

            }
        }



        /*****************************************************
         * 
         * loadArmorData
         * 
        *****************************************************/

        private void loadArmorData()
        {
            string startOffset = "00659B";
            string offset;
            int dataSize;
            for (var i = 0; i < maxArmors; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("11", i));    // 0A is 10 in hex
                //charOffset[i] = offset;

                //this.charNames[i] = this.romData.readDataString(offset, 1);
                dataSize = 2;
                armorGuard[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                armorCost[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                armorUsers[i] = readDataByte(offset, dataSize, true);
                armorUsers[i] = hex2bin(armorUsers[i]);
                offset = hexAdd(offset, dataSize.ToString());
                armorThunderRes[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                armorUnknownRes1[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                armorUnknownRes2[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                armorFireRes[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                armorIceRes[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                armorVacuumRes[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                armorMagicRes[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 3;
                armorNameOffset[i] = readDataByte(offset, dataSize, true);
                armorName[i] = buildString(armorNameOffset[i], 10);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 2;
                armorUnknown[i] = readDataByte(offset, dataSize, true);
                armorUnknownBin[i] = hex2bin(armorUnknown[i], 2);

            }
        }



        /*****************************************************
         * 
         * loadSpellData
         * 
        *****************************************************/

        private void loadSpellData()
        {
            string startOffset = "007018";
            string offset;
            int dataSize;
            for (var i = 0; i < maxSpells; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("0C", i));    // 0C is 12 in hex

                dataSize = 2;
                spellPower[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                spellTarget[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 2;
                spellCost[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());


                dataSize = 1;
                spellDomain[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                spellElement[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                spellUnknown1[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                spellUnknown2[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 3;
                spellNameOffset[i] = readDataByte(offset, dataSize, true);
                spellName[i] = buildString(spellNameOffset[i], 10);
            }
        }



        /*****************************************************
         * 
         * loadMonsterData
         * 
        *****************************************************/

        private void loadMonsterData()
        {
            string startOffset = "0072F4";
            string offset;
            int dataSize;
            for (var i = 0; i < maxMonsters; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("2A", i));    // 2A is 42 in hex

                dataSize = 1;
                monsterUnknown1[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 2;
                monsterMaxHP[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                monsterMaxMP[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                monsterPower[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                monsterGuard[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                monsterMagic[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                monsterSpeed[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());


                string offset2 = offset;

                for (var j = 0; j < maxMonsterSpells; j += 1)
                {
                    offset2 = hexAdd(offset, this.hexMult("1", j));
                    monsterSpells[i, j] = readDataInt(offset2, dataSize, true);
                    offset2 = hexAdd(offset2, "08");
                    monsterSpellsChance[i, j] = readDataInt(offset2, dataSize, true);
                }
                offset = hexAdd(offset, "10");

                dataSize = 1;
                for (var j = 0; j < maxElements; j += 1)
                {
                    monsterResistances[i, j] = readDataInt(offset, dataSize, true);
                    offset = hexAdd(offset, dataSize.ToString());
                }

                dataSize = 2;
                monsterGold[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                monsterDropSet[i] = readDataInt(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                monsterUnknown2[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                monsterRunFlag[i] = readDataByte(offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 3;
                monsterNameOffset[i] = readDataByte(offset, dataSize, true);
                monsterName[i] = buildString(monsterNameOffset[i], 10);



            }
        }



        /*****************************************************
         * 
         * loadDropSetData
         * 
        *****************************************************/

        private void loadDropSetData()
        {
            string startOffset = "008A18";
            string offset = startOffset;
            int dataSize = 1;
            for (var i = 0; i < maxDropSets; i += 1)
            {
                for (var j = 0; j < maxItemsPerDropSet; j += 1)
                {
                    dropSets[i, j] = readDataInt(offset, dataSize, true);
                    offset = hexAdd(offset, dataSize.ToString());
                }
            }
        }



        /*****************************************************
         * 
         * loadExperienceTable
         * 
        *****************************************************/

        private void loadExperienceTable()
        {
            string startOffset = "008CC8";
            string offset = startOffset;
            int dataSize = 3;
            for (var i = 0; i < maxExperienceTable; i += 1)
            {
                experienceTable[i] = readDataInt(offset, dataSize, true);
                Debug.WriteLine(offset + "  >> loading level " + i.ToString() + ":  " + experienceTable[i].ToString());
                offset = hexAdd(offset, dataSize.ToString());
            }
        }

        /*****************************************************
         * 
         * saveDropSetData
         * 
        *****************************************************/

        private void saveDropSetData()
        {
            string startOffset = "008A18";
            string offset = startOffset;
            int dataSize = 1;
            for (var i = 0; i < maxDropSets; i += 1)
            {
                for (var j = 0; j < maxItemsPerDropSet; j += 1)
                {
                    writeDataInt(dropSets[i, j], offset, dataSize, true);
                    offset = hexAdd(offset, dataSize.ToString());
                }
            }
        }

        //public int[,] dropSets = new int[maxDropSets, maxItemsPerDropSet];


        /*****************************************************
         * 
         * saveCharacterData
         * 
        *****************************************************/

        private void saveCharacterData()
        {
            string startOffset = "00623F";
            string offset;
            int dataSize;


            for (var i = 0; i < maxCharacters; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("12", i));    // 12 is 18 in hex
                charOffset[i] = offset;

                //this.charNames[i] = this.romData.readDataString(offset, 1);
                dataSize = 2;
                writeDataInt(charHPBase[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charMPBase[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                writeDataInt(charPowerBase[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charGuardBase[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charMagicBase[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charSpeedBase[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());


                dataSize = 1;
                writeDataInt(charHPGrowth[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charMPGrowth[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charPowerGrowth[i], offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charGuardGrowth[i], offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charMagicGrowth[i], offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charSpeedGrowth[i], offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());

                writeDataInt(charStartWeapon[i] + 100, offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charStartArmor[i] + 151, offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charStartAccessory[i] + 151, offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(charStartExp[i], offset, dataSize, false);
                offset = hexAdd(offset, dataSize.ToString());

                saveLearnData(i);
            }
        }



        /*****************************************************
         * 
         * saveLearnData
         * 
        *****************************************************/
        private void saveLearnData(int characterIndex)
        {
            string startOffset = "0062BD";
            startOffset = hexAdd(startOffset, this.hexMult("20", characterIndex)); // 20 is 32 in hex
            string offset;
            int dataSize = 1;

            for (var i = 0; i < maxLearn; i += 1)
            {
                offset = hexAdd(startOffset, this.hexMult("1", i));
                writeDataInt(charLearnSpells[characterIndex, i], offset, dataSize, true);
                offset = hexAdd(offset, "10");
                writeDataInt(charLearnLevels[characterIndex, i], offset, dataSize, true);
            }

        }



        /*****************************************************
         * 
         * saveItemData
         * 
        *****************************************************/

        private void saveItemData()
        {
            string startOffset = "006C94";
            string offset;
            int dataSize;
            for (var i = 0; i < maxItems; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("09", i));
                //charOffset[i] = offset;

                //this.charNames[i] = this.romData.readDataString(offset, 1);
                dataSize = 1;
                writeDataByte(itemTarget[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataByte(itemSingleUse[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 2;
                writeDataInt(itemCost[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 1;
                writeDataByte(bin2hex(itemUsers[i]), offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                writeDataByte(itemSellRatio[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 3;
                writeDataByte(itemNameOffset[i], offset, dataSize, true);

            }
        }



        /*****************************************************
         * 
         * saveSpellData
         * 
        *****************************************************/

        private void saveSpellData()
        {
            string startOffset = "007018";
            string offset;
            int dataSize;
            for (var i = 0; i < maxSpells; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("0C", i));    // 0C is 12 in hex

                dataSize = 2;
                writeDataInt(spellPower[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                writeDataByte(spellTarget[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 2;
                writeDataInt(spellCost[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());


                dataSize = 1;
                writeDataByte(spellDomain[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataByte(spellElement[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataByte(spellUnknown1[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataByte(spellUnknown2[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 3;
                writeDataByte(spellNameOffset[i], offset, dataSize, true);
            }
        }



        /*****************************************************
         * 
         * saveMonsterData
         * 
        *****************************************************/

        private void saveMonsterData()
        {
            string startOffset = "0072F4";
            string offset;
            int dataSize;
            for (var i = 0; i < maxMonsters; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("2A", i));    // 2A is 42 in hex

                dataSize = 1;
                writeDataByte(monsterUnknown1[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 2;
                writeDataInt(monsterMaxHP[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(monsterMaxMP[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(monsterPower[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(monsterGuard[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                writeDataInt(monsterMagic[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(monsterSpeed[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());


                string offset2 = offset;

                for (var j = 0; j < maxMonsterSpells; j += 1)
                {
                    offset2 = hexAdd(offset, this.hexMult("1", j));
                    writeDataInt(monsterSpells[i, j], offset2, dataSize, true);
                    offset2 = hexAdd(offset2, "08");
                    writeDataInt(monsterSpellsChance[i, j], offset2, dataSize, true);
                }
                offset = hexAdd(offset, "10");

                dataSize = 1;
                for (var j = 0; j < maxElements; j += 1)
                {
                    writeDataInt(monsterResistances[i, j], offset, dataSize, true);
                    offset = hexAdd(offset, dataSize.ToString());
                }

                dataSize = 2;
                writeDataInt(monsterGold[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                writeDataInt(monsterDropSet[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataByte(monsterUnknown2[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataByte(monsterRunFlag[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 3;
                writeDataByte(monsterNameOffset[i], offset, dataSize, true);


            }
        }



        /*****************************************************
         * 
         * saveWeaponData
         * 
        *****************************************************/

        private void saveWeaponData()
        {
            string startOffset = "00639D";
            string offset;
            int dataSize;
            for (var i = 0; i < maxWeapons; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("0A", i));    // 0A is 10 in hex
                //charOffset[i] = offset;
                //this.charNames[i] = this.romData.readDataString(offset, 1);
                dataSize = 2;
                writeDataInt(weaponPower[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(weaponCost[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 1;
                //weaponUsers[i] = bin2hex(weaponUsers[i]);
                writeDataByte(bin2hex(weaponUsers[i]), offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                
                dataSize = 3;
                writeDataByte(weaponNameOffset[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());

                dataSize = 2;
                writeDataByte(weaponUnknown[i], offset, dataSize, true);

            }
        }



        /*****************************************************
         * 
         * saveArmorData
         * 
        *****************************************************/

        private void saveArmorData()
        {
            string startOffset = "00659B";
            string offset;
            int dataSize;
            for (var i = 0; i < maxArmors; i += 1)
            {

                offset = hexAdd(startOffset, this.hexMult("11", i));    // 0A is 10 in hex
                //charOffset[i] = offset;

                //this.charNames[i] = this.romData.readDataString(offset, 1);
                dataSize = 2;
                writeDataInt(armorGuard[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(armorCost[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 1;
                writeDataByte(bin2hex(armorUsers[i]), offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(armorThunderRes[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(armorUnknownRes1[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(armorUnknownRes2[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(armorFireRes[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(armorIceRes[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(armorVacuumRes[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                writeDataInt(armorMagicRes[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 3;
                writeDataByte(armorNameOffset[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
                dataSize = 2;
                writeDataByte(armorUnknown[i], offset, dataSize, true);

            }
        }



        /*****************************************************
         * 
         * saveExperienceTable
         * 
        *****************************************************/

        private void saveExperienceTable()
        {
            string startOffset = "008CC8";
            string offset = startOffset;
            int dataSize = 3;
            for (var i = 0; i < maxExperienceTable; i += 1)
            {
                Debug.WriteLine(offset + "  >> writing level " + i.ToString() + ":  " + experienceTable[i].ToString());
                writeDataInt(experienceTable[i], offset, dataSize, true);
                offset = hexAdd(offset, dataSize.ToString());
            }
        }



        /*****************************************************
         * 
         * setupCharacterMap
         * 
        *****************************************************/

        private void setupCharacterMap() 
        {
            

            characterMap.Add("00", "0"); characterMap.Add("01", "1"); characterMap.Add("02", "2"); characterMap.Add("03", "3"); characterMap.Add("04", "4"); characterMap.Add("05", "5");
            characterMap.Add("06", "6"); characterMap.Add("07", "7"); characterMap.Add("08", "8"); characterMap.Add("09", "9"); characterMap.Add("0D", " "); 
            characterMap.Add("20", "A"); characterMap.Add("21", "B"); characterMap.Add("22", "C"); characterMap.Add("23", "D"); characterMap.Add("24", "E"); characterMap.Add("25", "F");
            characterMap.Add("26", "G"); characterMap.Add("27", "H"); characterMap.Add("28", "I"); characterMap.Add("29", "J"); characterMap.Add("2A", "K"); characterMap.Add("2B", "L"); 
            characterMap.Add("2C", "M"); characterMap.Add("2D", "N"); characterMap.Add("2E", "O"); characterMap.Add("2F", "P"); characterMap.Add("30", "Q"); characterMap.Add("31", "R");
            characterMap.Add("32", "S"); characterMap.Add("33", "T"); characterMap.Add("34", "U"); characterMap.Add("35", "V"); characterMap.Add("36", "W"); characterMap.Add("37", "X");
            characterMap.Add("38", "Y"); characterMap.Add("39", "Z");
            characterMap.Add("3A", "a"); characterMap.Add("3B", "b"); characterMap.Add("3C", "c"); characterMap.Add("3D", "d"); characterMap.Add("3E", "e"); characterMap.Add("3F", "f");
            characterMap.Add("40", "g"); characterMap.Add("41", "h"); characterMap.Add("42", "i"); characterMap.Add("43", "j"); characterMap.Add("44", "k"); characterMap.Add("45", "l");
            characterMap.Add("46", "m"); characterMap.Add("47", "n"); characterMap.Add("48", "o"); characterMap.Add("49", "p"); characterMap.Add("4A", "q"); characterMap.Add("4B", "r");
            characterMap.Add("4C", "s"); characterMap.Add("4D", "t"); characterMap.Add("4E", "u"); characterMap.Add("4F", "v"); characterMap.Add("50", "w"); characterMap.Add("51", "x");
            characterMap.Add("52", "y"); characterMap.Add("53", "z");
            characterMap.Add("56", "?"); characterMap.Add("57", "1"); characterMap.Add("58", "2"); characterMap.Add("59", "3"); characterMap.Add("5A", ":"); characterMap.Add("5B", ";");
            characterMap.Add("66", "'"); characterMap.Add("67", "\""); characterMap.Add("68", "-"); characterMap.Add("69", ","); characterMap.Add("6A", ".");
            
            characterMap.Add("6B", "<HT>"); characterMap.Add("6C", "<SB>"); characterMap.Add("6D", "<CR>"); characterMap.Add("6E", "<MK>"); characterMap.Add("6F", "<HA>");
            characterMap.Add("70", "<AX>"); characterMap.Add("71", "<SW>"); characterMap.Add("72", "<KN>"); characterMap.Add("73", "<ST>"); characterMap.Add("74", "<AR>");
            characterMap.Add("75", "<SH>"); characterMap.Add("76", "<CK>"); 
            characterMap.Add("7A", "<AM>");
            characterMap.Add("7D", "<RD>"); characterMap.Add("7E", "<ML>"); characterMap.Add("7F", "<RB>");

            characterMap.Add("F7", "<END>");

        }



        /*****************************************************
         * 
         * getCharacter
         * 
        *****************************************************/
        private string getCharacter(string hex)
        {
            string result;
            if (characterMap.TryGetValue(hex, out result))
            {
                return result;
            }
            else
            {
                return " ";
            }

        }



        /*****************************************************
         * 
         * buildString
         * 
        *****************************************************/

        public string buildString(string offset, int numBytes)
        {
            string auxbyte, bytes, result = "", character;
            offset = getRealOffset(offset);
            bytes = readDataByte(offset, numBytes, true);
            bytes = littleEndian(bytes);
            int i;
            for (i = 0; i < numBytes*2; i+=2)
            {
                auxbyte = bytes.Substring(i, 2);
                if (auxbyte == endOfNameByte)
                    break;
                character = getCharacter(auxbyte);
                result += character;
            }
            return result;
        }



        /*****************************************************
         * 
         * getRealOffset
         * 
        *****************************************************/

        private string getRealOffset(string offset)
        {
            int v1 = int.Parse(offset, System.Globalization.NumberStyles.HexNumber);
            v1 = v1 & 0x0FFFFF;
            offset = v1.ToString("X");

            return offset;

        }



        /*****************************************************
         * 
         * littleEndian
         * 
        *****************************************************/
        static string littleEndian(string num)
        {
            string newnum = "";
            if (num.Length % 2 == 1) 
            {
                num = "0" + num;
            }
            for (var i = 0; i < num.Length; i += 2)
            {
                newnum = num.Substring(i, 2) + newnum;
            }
            return newnum;
        }



        /*****************************************************
         * 
         * hex2bin
         * 
        *****************************************************/

        static string hex2bin(string hexstring)
        {
            string res = "";
            res = Convert.ToString(Convert.ToInt32(hexstring, 16), 2);
            int dif = 8 - res.Length;

            for (var i = 0; i < dif; i++)
            {
                res = "0" + res;
            }
            return res;
        }

        static string hex2bin(string hexstring, int bytes)
        {
            string res = "";
            res = Convert.ToString(Convert.ToInt32(hexstring, 16), 2);
            int dif = (8 * bytes) - res.Length;

            for (var i = 0; i < dif; i++)
            {
                res = "0" + res;
            }
            return res;
        }



        /*****************************************************
         * 
         * bin2hex
         * 
        *****************************************************/

        static string bin2hex(string binstring)
        {
            string res = Convert.ToInt32(binstring, 2).ToString("X");

            if (res.Length % 2 == 1)
            {
                res = "0" + res;
            }
            return res;
        }
    }
}
