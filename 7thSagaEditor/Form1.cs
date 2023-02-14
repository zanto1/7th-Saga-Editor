using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using SevenSagaEditor.RomInfo;

namespace SevenSagaEditor
{
    public partial class Form1 : Form
    {
        private Rom romData;
        private string selectedRom;
        private bool justLoadedRom;            // prevents the program to do some things when it loads rom data
        private bool justLoadedLearnList;
        private int previousCharacterIndex;
        private int previousLearnIndex;
        private int previousSpellIndex;
        private int previousItemIndex;
        private int previousWeaponIndex;
        private int previousArmorIndex;
        private int previousDropSetIndex;
        private int previousExperienceTableIndex;
        private bool updateSpellListBool;
        private int previousMonsterIndex;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setFieldsState(false);
        }





        /*****************************************************
         * 
         * setFieldsState
         * 
        *****************************************************/

        private void setFieldsState(bool value)
        {
            this.cbCharacters.Enabled = value;
            this.numHPBase.Enabled = value;
            this.numHPGrowth.Enabled = value;
            this.numMPBase.Enabled = value;
            this.numMPGrowth.Enabled = value;
            this.numGuardBase.Enabled = value;
            this.numGuardGrowth.Enabled = value;
            this.numMagicBase.Enabled = value;
            this.numMagicGrowth.Enabled = value;
            this.numPowerGrowth.Enabled = value;
            this.numPowerBase.Enabled = value;
            this.numSpeedBase.Enabled = value;
            this.numSpeedGrowth.Enabled = value;
            this.numStartExp.Enabled = value;
            this.cbStartAccessory.Enabled = value;
            this.cbStartWeapon.Enabled = value;
            this.cbStartArmor.Enabled = value;
            this.cbLearnList.Enabled = value;
            this.cbLearnSpell.Enabled = value;
            this.numLearnLevel.Enabled = value;
            this.tabControl1.Enabled = value;
        }



        /*****************************************************
         * 
         * openRomToolStripMenuItem_Click
         * 
        *****************************************************/

        private void openRomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.DialogResult result = this.openFileOpenRom.ShowDialog(this);
                if (result.Equals(System.Windows.Forms.DialogResult.OK))
                {
                    selectedRom = this.openFileOpenRom.FileName;
                    this.Text = "7th Saga Editor - " + selectedRom;
                    //File.Copy(selectedRom, "7thSagaAux.sfc");
                    this.romData = new Rom(selectedRom);
                    this.Text = "7th Saga Editor - " + selectedRom;
                    justLoadedRom = true;
                    setFieldsState(true);


                    loadItemList();
                    //loadSpellFields();
                    cbItems.SelectedIndex = 0;
                    previousItemIndex = 0;

                    loadEquipmentFields();
                    loadLearnSpellFields();
                    this.cbCharacters.SelectedIndex = 0;
                    this.cbLearnList.SelectedIndex = 0;
                    previousCharacterIndex = 0;
                    //setupCharacterFields();
                    
                    loadSpellList();
                    //loadSpellFields();
                    cbSpells.SelectedIndex = 0;
                    previousSpellIndex = 0;

                    loadMonsterList();
                    cbMonsters.SelectedIndex = 0;
                    previousMonsterIndex = 0;

                    cbWeapons.SelectedIndex = 0;
                    previousWeaponIndex = 0;

                    cbArmors.SelectedIndex = 0;
                    previousArmorIndex = 0;

                    loadDropSetList();
                    cbDrops.SelectedIndex = 0;
                    previousDropSetIndex = 0;

                    fillUpExpTableList();
                    previousExperienceTableIndex = 0;
                    cbExpTableLevel.SelectedIndex = 0;

                    justLoadedRom = false;
                    updateSpellListBool = true;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("Error loading rom: " + x.ToString());
            }
        }



        /*****************************************************
         * 
         * loadCharacterFields
         * 
        *****************************************************/

        private void loadCharacterFields()
        {
            int characterIndex = this.cbCharacters.SelectedIndex;

            numHPBase.Value = this.romData.charHPBase[characterIndex];
            numHPGrowth.Value = this.romData.charHPGrowth[characterIndex];
            numMPBase.Value = this.romData.charMPBase[characterIndex];
            numMPGrowth.Value = this.romData.charMPGrowth[characterIndex];

            numPowerBase.Value = this.romData.charPowerBase[characterIndex];
            numPowerGrowth.Value = this.romData.charPowerGrowth[characterIndex];

            numGuardBase.Value = this.romData.charGuardBase[characterIndex];
            numGuardGrowth.Value = this.romData.charGuardGrowth[characterIndex];
            numMagicBase.Value = this.romData.charMagicBase[characterIndex];
            numMagicGrowth.Value = this.romData.charMagicGrowth[characterIndex];
            numSpeedBase.Value = this.romData.charSpeedBase[characterIndex];
            numSpeedGrowth.Value = this.romData.charSpeedGrowth[characterIndex];
            numStartExp.Value = this.romData.charStartExp[characterIndex];

            //lbCharOffset.Text = this.romData.charOffset[characterIndex];
            lbCharOffset.Text = this.romData.weaponName[this.romData.charStartWeapon[characterIndex]];
            //lbCharOffset.Text = this.romData.weaponName[1];
            cbStartWeapon.SelectedIndex = this.romData.charStartWeapon[characterIndex];
            cbStartArmor.SelectedIndex = this.romData.charStartArmor[characterIndex];

            justLoadedLearnList = true;
            //
            loadLearnFields();
            this.cbLearnList.SelectedIndex = 0;
            previousLearnIndex = 0;
            justLoadedLearnList = false;
        }



        /*****************************************************
         * 
         * loadLearnFields
         * 
        *****************************************************/

        private void loadLearnFields()
        {
            int characterIndex = this.cbCharacters.SelectedIndex;
            cbLearnList.Items.Clear(); 
            string itemName;
            for (var i = 0; i < Rom.maxLearn; i++)
            {
                //itemName = this.romData.charLearnSpells[characterIndex, i] + " - " + this.romData.charLearnLevels[characterIndex, i].ToString();
                itemName = romData.spellName[romData.charLearnSpells[characterIndex, i]] + "   at level " + this.romData.charLearnLevels[characterIndex, i].ToString();
                
                cbLearnList.Items.Insert(i, itemName);

            }
        }




        private void cbLearnSpell_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updateSpellListBool)
            {
                updateLearnListItem();
            }
        }

        private void numLearnLevel_ValueChanged(object sender, EventArgs e)
        {
            if (updateSpellListBool)
            {
                updateLearnListItem();
            }
        }

        private void updateLearnListItem()
        {
            string itemName;
            int characterIndex = this.cbCharacters.SelectedIndex;
            if (!justLoadedLearnList)
            {
                saveLearnDetailFields();
                // in order to update the list based on changes done to the learn detail fields
                itemName = romData.spellName[cbLearnSpell.SelectedIndex] + "   at level " + numLearnLevel.Value.ToString();
                cbLearnList.Items[cbLearnList.SelectedIndex] = itemName;
            }
        }

        private void cbLearnList_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateSpellListBool = false;
            int characterIndex = this.cbCharacters.SelectedIndex;
            numLearnLevel.Value = romData.charLearnLevels[characterIndex, this.cbLearnList.SelectedIndex];
            cbLearnSpell.SelectedIndex = romData.charLearnSpells[characterIndex, this.cbLearnList.SelectedIndex];
            previousLearnIndex = this.cbLearnList.SelectedIndex;
            updateSpellListBool = true;

        }



        /*****************************************************
         * 
         * saveLearnDetailFields
         * 
        *****************************************************/

        private void saveLearnDetailFields()
        {
            int characterIndex = this.cbCharacters.SelectedIndex;
            int learnIndex = this.cbLearnList.SelectedIndex;

            this.romData.charLearnLevels[characterIndex, learnIndex] = Convert.ToInt32(numLearnLevel.Value);
            this.romData.charLearnSpells[characterIndex, learnIndex] = Convert.ToInt32(cbLearnSpell.SelectedIndex);
            
        }



        /*****************************************************
         * 
         * saveCharacterFields
         * 
        *****************************************************/

        private void saveCharacterFields()
        {
            int characterIndex = previousCharacterIndex;

            this.romData.charHPBase[characterIndex] = Convert.ToInt32(numHPBase.Value);
            this.romData.charHPGrowth[characterIndex] = Convert.ToInt32(numHPGrowth.Value);
            this.romData.charMPBase[characterIndex] = Convert.ToInt32(numMPBase.Value);
            this.romData.charMPGrowth[characterIndex] = Convert.ToInt32(numMPGrowth.Value);

            this.romData.charPowerBase[characterIndex] = Convert.ToInt32(numPowerBase.Value);
            this.romData.charPowerGrowth[characterIndex] = Convert.ToInt32(numPowerGrowth.Value);
            this.romData.charGuardBase[characterIndex] = Convert.ToInt32(numGuardBase.Value);
            this.romData.charGuardGrowth[characterIndex] = Convert.ToInt32(numGuardGrowth.Value);
            this.romData.charMagicBase[characterIndex] = Convert.ToInt32(numMagicBase.Value);
            this.romData.charMagicGrowth[characterIndex] = Convert.ToInt32(numMagicGrowth.Value);
            this.romData.charSpeedBase[characterIndex] = Convert.ToInt32(numSpeedBase.Value);
            this.romData.charSpeedGrowth[characterIndex] = Convert.ToInt32(numSpeedGrowth.Value);
            this.romData.charStartExp[characterIndex] = Convert.ToInt32(numStartExp.Value);
        }



        /*****************************************************
         * 
         * cbCharacters_SelectedIndexChanged
         * 
        *****************************************************/

        private void cbCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            int characterIndex = this.cbCharacters.SelectedIndex;
            if (!justLoadedRom)
            {
                saveCharacterFields();
            }
            loadCharacterFields();

            previousCharacterIndex = this.cbCharacters.SelectedIndex;
        }



        /*****************************************************
         * 
         * loadItemList
         * 
        *****************************************************/

        private void loadItemList()
        {
            String hexstr;
            for (var i = 0; i < Rom.maxItems; i++)
            {
                //hexstr = i.ToString("X" + (2).ToString());
                //cbItems.Items.Insert(i, hexstr + " > " + this.romData.itemName[i]);
                cbItems.Items.Insert(i, this.romData.itemName[i]);
                fillDropSetLists(this.romData.itemName[i]);
            }
        }



        /*****************************************************
         * 
         * loadEquipmentFields
         * 
        *****************************************************/

        private void loadEquipmentFields()
        {
            for (var i = 0; i < Rom.maxWeapons; i++)
            {
                cbStartWeapon.Items.Insert(i, this.romData.weaponName[i]);
                cbWeapons.Items.Insert(i, this.romData.weaponName[i]);
                fillDropSetLists(this.romData.weaponName[i]);

            }
            for (var i = 0; i < Rom.maxArmors; i++)
            {
                cbStartArmor.Items.Insert(i, this.romData.armorName[i]);
                cbArmors.Items.Insert(i, this.romData.armorName[i]);
                fillDropSetLists(this.romData.armorName[i]);
            }
        }



        /*****************************************************
         * 
         * loadLearnSpellFields
         * 
        *****************************************************/

        private void loadLearnSpellFields()
        {
            for (var i = 0; i < Rom.maxSpells; i++)
            {
                cbLearnSpell.Items.Insert(i, this.romData.spellName[i]);

            }
        }

        /*************************************************************
         * ***********************************************************
         * ***********************************************************
         * 
         * SPELL STUFF
         * 
         * ***********************************************************
         * ***********************************************************
         * ***********************************************************/





        /*****************************************************
         * 
         * loadSpellList
         * 
        *****************************************************/

        private void loadSpellList()
        {
            for (var i = 0; i < Rom.maxSpells; i++)
            {
                cbSpells.Items.Insert(i, this.romData.spellName[i]);
                cbMonsterSpell1.Items.Insert(i, this.romData.spellName[i]);
                cbMonsterSpell2.Items.Insert(i, this.romData.spellName[i]);
                cbMonsterSpell3.Items.Insert(i, this.romData.spellName[i]);
                cbMonsterSpell4.Items.Insert(i, this.romData.spellName[i]);
                cbMonsterSpell5.Items.Insert(i, this.romData.spellName[i]);
                cbMonsterSpell6.Items.Insert(i, this.romData.spellName[i]);
                cbMonsterSpell7.Items.Insert(i, this.romData.spellName[i]);
                cbMonsterSpell8.Items.Insert(i, this.romData.spellName[i]);
            }
        }



        /*****************************************************
         * 
         * loadSpellFields
         * 
        *****************************************************/
        private void loadSpellFields()
        {
            int spellIndex = this.cbSpells.SelectedIndex;
            numSpellPower.Value = this.romData.spellPower[spellIndex];
            numSpellMpCost.Value = this.romData.spellCost[spellIndex];
            txtSpellUnknown1.Text = this.romData.spellUnknown1[spellIndex];
            txtSpellUnknown2.Text = this.romData.spellUnknown2[spellIndex];

            switch (this.romData.spellDomain[spellIndex])
            {
                case "00":
                    cbSpellDomain.SelectedIndex = 0;
                    break;
                case "01":
                    cbSpellDomain.SelectedIndex = 1;
                    break;
                case "02":
                    cbSpellDomain.SelectedIndex = 2;
                    break;
            }
            switch (this.romData.spellTarget[spellIndex])
            {
                case "00":
                    cbSpellTarget.SelectedIndex = 0;
                    break;
                case "01":
                    cbSpellTarget.SelectedIndex = 1;
                    break;
                case "02":
                    cbSpellTarget.SelectedIndex = 2;
                    break;
                case "04":
                    cbSpellTarget.SelectedIndex = 3;
                    break;
            }
            switch (this.romData.spellElement[spellIndex])
            {
                case "00":
                    cbSpellElement.SelectedIndex = 0;
                    break;
                case "01":
                    cbSpellElement.SelectedIndex = 1;
                    break;
                case "04":
                    cbSpellElement.SelectedIndex = 2;
                    break;
                case "05":
                    cbSpellElement.SelectedIndex = 3;
                    break;
                case "06":
                    cbSpellElement.SelectedIndex = 4;
                    break;
                case "07":
                    cbSpellElement.SelectedIndex = 5;
                    break;
            }
        }



        /*****************************************************
         * 
         * saveSpellFields
         * 
        *****************************************************/

        private void saveSpellFields()
        {
            int spellIndex = previousSpellIndex;
            this.romData.spellPower[spellIndex] = Convert.ToInt32(numSpellPower.Value);
            this.romData.spellCost[spellIndex] = Convert.ToInt32(numSpellMpCost.Value);
            this.romData.spellUnknown1[spellIndex] = txtSpellUnknown1.Text;
            this.romData.spellUnknown2[spellIndex] = txtSpellUnknown2.Text;

            switch (cbSpellDomain.SelectedIndex)
            {
                case 0:
                    this.romData.spellDomain[spellIndex] = "00";
                    break;
                case 1:
                    this.romData.spellDomain[spellIndex] = "01";
                    break;
                case 2:
                    this.romData.spellDomain[spellIndex] = "02";
                    break;
            }
            switch (cbSpellTarget.SelectedIndex)
            {
                case 0:
                    this.romData.spellTarget[spellIndex] = "00";
                    break;
                case 1:
                    this.romData.spellTarget[spellIndex] = "01";
                    break;
                case 2:
                    this.romData.spellTarget[spellIndex] = "02";
                    break;
                case 3:
                    this.romData.spellTarget[spellIndex] = "04";
                    break;
            }
            switch (cbSpellElement.SelectedIndex)
            {
                case 0:
                    this.romData.spellElement[spellIndex] = "00";
                    break;
                case 1:
                    this.romData.spellElement[spellIndex] = "01";
                    break;
                case 2:
                    this.romData.spellElement[spellIndex] = "04";
                    break;
                case 3:
                    this.romData.spellElement[spellIndex] = "05";
                    break;
                case 4:
                    this.romData.spellElement[spellIndex] = "06";
                    break;
                case 5:
                    this.romData.spellElement[spellIndex] = "07";
                    break;
            }
        }



        /*****************************************************
         * 
         * cbSpells_SelectedIndexChanged
         * 
        *****************************************************/
        private void cbSpells_SelectedIndexChanged(object sender, EventArgs e)
        {

            int spellIndex = this.cbSpells.SelectedIndex;
            if (!justLoadedRom)
            {
                saveSpellFields();
            }
            loadSpellFields();

            previousSpellIndex = this.cbSpells.SelectedIndex;

        }



        /*************************************************************
         * ***********************************************************
         * ***********************************************************
         * 
         * ENEMY STUFF
         * 
         * ***********************************************************
         * ***********************************************************
         * ***********************************************************/


        /*****************************************************
         * 
         * loadMonsterList
         * 
        *****************************************************/

        private void loadMonsterList()
        {
            for (var i = 0; i < Rom.maxMonsters; i++)
            {
                cbMonsters.Items.Insert(i, this.romData.monsterName[i]);

            }
        }



        /*****************************************************
         * 
         * loadMonsterFields
         * 
        *****************************************************/

        private void loadMonsterFields()
        {
            int monsterIndex = this.cbMonsters.SelectedIndex;

            numMonsterHp.Value = this.romData.monsterMaxHP[monsterIndex];
            numMonsterMp.Value = this.romData.monsterMaxMP[monsterIndex];
            numMonsterPower.Value = this.romData.monsterPower[monsterIndex];
            numMonsterGuard.Value = this.romData.monsterGuard[monsterIndex];
            numMonsterMagic.Value = this.romData.monsterMagic[monsterIndex];
            numMonsterSpeed.Value = this.romData.monsterSpeed[monsterIndex];
            numMonsterGold.Value = this.romData.monsterGold[monsterIndex];

            cbMonsterSpell1.SelectedIndex = this.romData.monsterSpells[monsterIndex, 0];
            cbMonsterSpell2.SelectedIndex = this.romData.monsterSpells[monsterIndex, 1];
            cbMonsterSpell3.SelectedIndex = this.romData.monsterSpells[monsterIndex, 2];
            cbMonsterSpell4.SelectedIndex = this.romData.monsterSpells[monsterIndex, 3];
            cbMonsterSpell5.SelectedIndex = this.romData.monsterSpells[monsterIndex, 4];
            cbMonsterSpell6.SelectedIndex = this.romData.monsterSpells[monsterIndex, 5];
            cbMonsterSpell7.SelectedIndex = this.romData.monsterSpells[monsterIndex, 6];
            cbMonsterSpell8.SelectedIndex = this.romData.monsterSpells[monsterIndex, 7];

            numMonsterSpellOdds1.Value = this.romData.monsterSpellsChance[monsterIndex, 0];
            numMonsterSpellOdds2.Value = this.romData.monsterSpellsChance[monsterIndex, 1];
            numMonsterSpellOdds3.Value = this.romData.monsterSpellsChance[monsterIndex, 2];
            numMonsterSpellOdds4.Value = this.romData.monsterSpellsChance[monsterIndex, 3];
            numMonsterSpellOdds5.Value = this.romData.monsterSpellsChance[monsterIndex, 4];
            numMonsterSpellOdds6.Value = this.romData.monsterSpellsChance[monsterIndex, 5];
            numMonsterSpellOdds7.Value = this.romData.monsterSpellsChance[monsterIndex, 6];
            numMonsterSpellOdds8.Value = this.romData.monsterSpellsChance[monsterIndex, 7];

            numMonsterDropSet.Value = this.romData.monsterDropSet[monsterIndex];
            if (this.romData.monsterRunFlag[monsterIndex] == "01")
            {
                chMonsterCanRun.Checked = true;
            }
            else
            {
                chMonsterCanRun.Checked = false;
            }
            txtMonsterUnknown1.Text = this.romData.monsterUnknown1[monsterIndex];
            txtMonsterUnknown2.Text = this.romData.monsterUnknown2[monsterIndex];

            numMonsterResThunder.Value = this.romData.monsterResistances[monsterIndex, 0];
            numMonsterResUnknown1.Value = this.romData.monsterResistances[monsterIndex, 1];
            numMonsterResUnknown2.Value = this.romData.monsterResistances[monsterIndex, 2];
            numMonsterResFire.Value = this.romData.monsterResistances[monsterIndex, 3];
            numMonsterResIce.Value = this.romData.monsterResistances[monsterIndex, 4];
            numMonsterResVacuum.Value = this.romData.monsterResistances[monsterIndex, 5];
            numMonsterResMagic.Value = this.romData.monsterResistances[monsterIndex, 6];

        }



        /*****************************************************
         * 
         * saveMonsterFields
         * 
        *****************************************************/

        private void saveMonsterFields()
        {
            int monsterIndex = previousMonsterIndex;

            this.romData.monsterMaxHP[monsterIndex] = Convert.ToInt32(numMonsterHp.Value);
            this.romData.monsterMaxMP[monsterIndex] = Convert.ToInt32(numMonsterMp.Value);
            this.romData.monsterPower[monsterIndex] = Convert.ToInt32(numMonsterPower.Value);
            this.romData.monsterGuard[monsterIndex] = Convert.ToInt32(numMonsterGuard.Value);
            this.romData.monsterMagic[monsterIndex] = Convert.ToInt32(numMonsterMagic.Value);
            this.romData.monsterSpeed[monsterIndex] = Convert.ToInt32(numMonsterSpeed.Value);
            this.romData.monsterGold[monsterIndex] = Convert.ToInt32(numMonsterGold.Value);

            this.romData.monsterSpells[monsterIndex, 0] = cbMonsterSpell1.SelectedIndex;
            this.romData.monsterSpells[monsterIndex, 1] = cbMonsterSpell2.SelectedIndex;
            this.romData.monsterSpells[monsterIndex, 2] = cbMonsterSpell3.SelectedIndex;
            this.romData.monsterSpells[monsterIndex, 3] = cbMonsterSpell4.SelectedIndex;
            this.romData.monsterSpells[monsterIndex, 4] = cbMonsterSpell5.SelectedIndex;
            this.romData.monsterSpells[monsterIndex, 5] = cbMonsterSpell6.SelectedIndex;
            this.romData.monsterSpells[monsterIndex, 6] = cbMonsterSpell7.SelectedIndex;
            this.romData.monsterSpells[monsterIndex, 7] = cbMonsterSpell8.SelectedIndex;

            this.romData.monsterSpellsChance[monsterIndex, 0] = Convert.ToInt32(numMonsterSpellOdds1.Value);
            this.romData.monsterSpellsChance[monsterIndex, 1] = Convert.ToInt32(numMonsterSpellOdds2.Value);
            this.romData.monsterSpellsChance[monsterIndex, 2] = Convert.ToInt32(numMonsterSpellOdds3.Value);
            this.romData.monsterSpellsChance[monsterIndex, 3] = Convert.ToInt32(numMonsterSpellOdds4.Value);
            this.romData.monsterSpellsChance[monsterIndex, 4] = Convert.ToInt32(numMonsterSpellOdds5.Value);
            this.romData.monsterSpellsChance[monsterIndex, 5] = Convert.ToInt32(numMonsterSpellOdds6.Value);
            this.romData.monsterSpellsChance[monsterIndex, 6] = Convert.ToInt32(numMonsterSpellOdds7.Value);
            this.romData.monsterSpellsChance[monsterIndex, 7] = Convert.ToInt32(numMonsterSpellOdds8.Value);

            this.romData.monsterDropSet[monsterIndex] = Convert.ToInt32(numMonsterDropSet.Value);
            if (chMonsterCanRun.Checked)
            {
                this.romData.monsterRunFlag[monsterIndex] = "01";
            }
            else
            {
                this.romData.monsterRunFlag[monsterIndex] = "00";
            }

            this.romData.monsterUnknown1[monsterIndex] = txtMonsterUnknown1.Text;
            this.romData.monsterUnknown2[monsterIndex] = txtMonsterUnknown2.Text;

            this.romData.monsterResistances[monsterIndex, 0] = Convert.ToInt32(numMonsterResThunder.Value);
            this.romData.monsterResistances[monsterIndex, 1] = Convert.ToInt32(numMonsterResUnknown1.Value);
            this.romData.monsterResistances[monsterIndex, 2] = Convert.ToInt32(numMonsterResUnknown2.Value);
            this.romData.monsterResistances[monsterIndex, 3] = Convert.ToInt32(numMonsterResFire.Value);
            this.romData.monsterResistances[monsterIndex, 4] = Convert.ToInt32(numMonsterResIce.Value);
            this.romData.monsterResistances[monsterIndex, 5] = Convert.ToInt32(numMonsterResVacuum.Value);
            this.romData.monsterResistances[monsterIndex, 6] = Convert.ToInt32(numMonsterResMagic.Value);

        }




        /*****************************************************
         * 
         * numericUpDown1_ValueChanged
         * 
        *****************************************************/
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            lbMonsterExp.Text = Math.Ceiling(System.Convert.ToDouble(numMonsterGold.Value) * 2.201).ToString();
        }


        /*****************************************************
         * 
         * cbMonsters_SelectedIndexChanged
         * 
        *****************************************************/

        private void cbMonsters_SelectedIndexChanged(object sender, EventArgs e)
        {
            int monsterIndex = this.cbMonsters.SelectedIndex;
            if (!justLoadedRom)
            {
                saveMonsterFields();
            }
            loadMonsterFields();
            previousMonsterIndex = this.cbMonsters.SelectedIndex;
        }







        /*************************************************************
         * ***********************************************************
         * ***********************************************************
         * 
         * WEAPON STUFF
         * 
         * ***********************************************************
         * ***********************************************************
         * ***********************************************************/



        /*****************************************************
         * 
         * loadWeaponFields
         * 
        *****************************************************/

        private void loadWeaponFields()
        {
            int weaponIndex = this.cbWeapons.SelectedIndex;
            string canequipstr = this.romData.weaponUsers[weaponIndex];
            CheckBox checkbox;
            int equipIndex;
            numWeaponPower.Value = this.romData.weaponPower[weaponIndex];
            numWeaponCost.Value = this.romData.weaponCost[weaponIndex];
            txtWeaponUnknown.Text = this.romData.weaponUnknown[weaponIndex];

            checkbox = cbWeaponKamil;
            equipIndex = 7;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbWeaponOlvan;
            equipIndex = 6;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbWeaponEsuna;
            equipIndex = 5;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbWeaponWilme;
            equipIndex = 4;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbWeaponLux;
            equipIndex = 3;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbWeaponValsu;
            equipIndex = 2;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbWeaponLejes;
            equipIndex = 1;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
        }



        /*****************************************************
         * 
         * saveWeaponFields
         * 
        *****************************************************/

        private void saveWeaponFields()
        {
            int weaponIndex = previousWeaponIndex;
            string canequipstr = "";
            CheckBox checkbox;

            this.romData.weaponPower[weaponIndex] = Convert.ToInt32(numWeaponPower.Value);
            this.romData.weaponCost[weaponIndex] = Convert.ToInt32(numWeaponCost.Value);
            this.romData.weaponUnknown[weaponIndex] = txtWeaponUnknown.Text;

            checkbox = cbWeaponKamil;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr; 
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbWeaponOlvan;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbWeaponEsuna;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbWeaponWilme;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbWeaponLux;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbWeaponValsu;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbWeaponLejes;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            canequipstr = "0" + canequipstr;
            this.romData.weaponUsers[weaponIndex] = canequipstr;

        }


        private void cbWeapons_SelectedIndexChanged(object sender, EventArgs e)            
        {
            if (!justLoadedRom)
            {
                saveWeaponFields();
            }
            loadWeaponFields();
            previousWeaponIndex = this.cbWeapons.SelectedIndex;
            
        }






        /*************************************************************
         * ***********************************************************
         * ***********************************************************
         * 
         * ARMOR STUFF
         * 
         * ***********************************************************
         * ***********************************************************
         * ***********************************************************/



        /*****************************************************
         * 
         * loadArmorFields
         * 
        *****************************************************/

        private void loadArmorFields()
        {
            int armorIndex = this.cbArmors.SelectedIndex;
            string canequipstr = this.romData.armorUsers[armorIndex];
            CheckBox checkbox;
            int equipIndex;
            numArmorGuard.Value = this.romData.armorGuard[armorIndex];
            numArmorCost.Value = this.romData.armorCost[armorIndex];
            txtArmorUnknown.Text = this.romData.armorUnknown[armorIndex];

            numArmorResThunder.Value = this.romData.armorThunderRes[armorIndex];
            numArmorResUnknown1.Value = this.romData.armorUnknownRes1[armorIndex];
            numArmorResUnknown2.Value = this.romData.armorUnknownRes2[armorIndex];
            numArmorResFire.Value = this.romData.armorFireRes[armorIndex];
            numArmorResIce.Value = this.romData.armorIceRes[armorIndex];
            numArmorResVacuum.Value = this.romData.armorVacuumRes[armorIndex];
            numArmorResMagic.Value = this.romData.armorMagicRes[armorIndex];

            checkbox = cbArmorKamil;
            equipIndex = 7;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbArmorOlvan;
            equipIndex = 6;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbArmorEsuna;
            equipIndex = 5;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbArmorWilme;
            equipIndex = 4;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbArmorLux;
            equipIndex = 3;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbArmorValsu;
            equipIndex = 2;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbArmorLejes;
            equipIndex = 1;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
        }



        /*****************************************************
         * 
         * saveArmorFields
         * 
        *****************************************************/

        private void saveArmorFields()
        {
            int armorIndex = previousArmorIndex;
            string canequipstr = "";
            CheckBox checkbox;

            this.romData.armorGuard[armorIndex] = Convert.ToInt32(numArmorGuard.Value);
            this.romData.armorCost[armorIndex] = Convert.ToInt32(numArmorCost.Value);
            this.romData.armorUnknown[armorIndex] = txtArmorUnknown.Text;

            this.romData.armorThunderRes[armorIndex] = Convert.ToInt32(numArmorResThunder.Value);
            this.romData.armorUnknownRes1[armorIndex] = Convert.ToInt32(numArmorResUnknown1.Value);
            this.romData.armorUnknownRes2[armorIndex] = Convert.ToInt32(numArmorResUnknown2.Value);
            this.romData.armorFireRes[armorIndex] = Convert.ToInt32(numArmorResFire.Value);
            this.romData.armorIceRes[armorIndex] = Convert.ToInt32(numArmorResIce.Value);
            this.romData.armorVacuumRes[armorIndex] = Convert.ToInt32(numArmorResVacuum.Value);
            this.romData.armorMagicRes[armorIndex] = Convert.ToInt32(numArmorResMagic.Value);

            checkbox = cbArmorKamil;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbArmorOlvan;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbArmorEsuna;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbArmorWilme;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbArmorLux;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbArmorValsu;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbArmorLejes;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            canequipstr = "0" + canequipstr;
            this.romData.armorUsers[armorIndex] = canequipstr;
        }



        /*****************************************************
         * 
         * cbArmors_SelectedIndexChanged
         * 
        *****************************************************/

        private void cbArmors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!justLoadedRom)
            {
                saveArmorFields();
            }
            loadArmorFields();
            previousArmorIndex = this.cbArmors.SelectedIndex;

        }

        private void txtArmorUnknown_TextChanged(object sender, EventArgs e)
        {
            int armorIndex = this.cbArmors.SelectedIndex;
            lbArmorUnknownBin.Text = hex2bin(txtArmorUnknown.Text, 2);
        }



        /*************************************************************
         * ***********************************************************
         * ***********************************************************
         * 
         * ITEM STUFF
         * 
         * ***********************************************************
         * ***********************************************************
         * ***********************************************************/



        /*****************************************************
         * 
         * loadItemFields
         * 
        *****************************************************/

        private void loadItemFields()
        {
            int itemIndex = this.cbItems.SelectedIndex;
            string canequipstr = this.romData.itemUsers[itemIndex];
            CheckBox checkbox;
            int equipIndex;
            numItemCost.Value = this.romData.itemCost[itemIndex];

            switch (this.romData.itemTarget[itemIndex])
            {
                case "00":
                    cbItemTarget.SelectedIndex = 0;
                    break;
                case "01":
                    cbItemTarget.SelectedIndex = 1;
                    break;
                case "02":
                    cbItemTarget.SelectedIndex = 2;
                    break;
                case "04":
                    cbItemTarget.SelectedIndex = 3;
                    break;
            }

            if (this.romData.itemSellRatio[itemIndex] == "01")
            {
                cbItemSellRatio.Checked = true;
            }
            else
            {
                cbItemSellRatio.Checked = false;
            }

            if (this.romData.itemSingleUse[itemIndex] == "01")
            {
                cbItemSingleUse.Checked = true;
            }
            else
            {
                cbItemSingleUse.Checked = false;
            }


            checkbox = cbItemKamil;
            equipIndex = 7;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbItemOlvan;
            equipIndex = 6;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbItemEsuna;
            equipIndex = 5;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbItemWilme;
            equipIndex = 4;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbItemLux;
            equipIndex = 3;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbItemValsu;
            equipIndex = 2;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
            checkbox = cbItemLejes;
            equipIndex = 1;
            if (canequipstr.Substring(equipIndex, 1) == "1")
            {
                checkbox.Checked = true;
            }
            else
            {
                checkbox.Checked = false;
            }
        }



        /*****************************************************
         * 
         * saveItemFields
         * 
        *****************************************************/

        private void saveItemFields()
        {
            int itemIndex = previousItemIndex;
            string canequipstr = "";
            CheckBox checkbox;

            this.romData.itemCost[itemIndex] = Convert.ToInt32(numItemCost.Value);

            switch (cbItemTarget.SelectedIndex)
            {
                case 0:
                    this.romData.itemTarget[itemIndex] = "00";
                    break;
                case 1:
                    this.romData.itemTarget[itemIndex] = "01";
                    break;
                case 2:
                    this.romData.itemTarget[itemIndex] = "02";
                    break;
                case 3:
                    this.romData.itemTarget[itemIndex] = "04";
                    break;
            }

            if (cbItemSellRatio.Checked == true)
            {
                this.romData.itemSellRatio[itemIndex] = "01";
                
            }
            else
            {
                this.romData.itemSellRatio[itemIndex] = "00";
            }

            if (cbItemSingleUse.Checked == true)
            {
                this.romData.itemSingleUse[itemIndex] = "01";
            }
            else
            {
                this.romData.itemSingleUse[itemIndex] = "00";
            }



            checkbox = cbItemKamil;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbItemOlvan;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbItemEsuna;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbItemWilme;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbItemLux;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbItemValsu;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            checkbox = cbItemLejes;
            if (checkbox.Checked)
            {
                canequipstr = "1" + canequipstr;
            }
            else
            {
                canequipstr = "0" + canequipstr;
            }
            canequipstr = "0" + canequipstr;
            this.romData.itemUsers[itemIndex] = canequipstr;
        }



        /*****************************************************
         * 
         * cbItems_SelectedIndexChanged
         * 
        *****************************************************/

        private void cbItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!justLoadedRom)
            {
                saveItemFields();
            }
            loadItemFields();
            previousItemIndex = this.cbItems.SelectedIndex;
        }

        




        /*************************************************************
         * ***********************************************************
         * ***********************************************************
         * 
         * OTHER THINGS STUFF
         * 
         * ***********************************************************
         * ***********************************************************
         * ***********************************************************/

        private void loadDropSetList()
        {
            for (var i=0;i<Rom.maxDropSets; i++)
            {
                cbDrops.Items.Insert(i, "Drop set #" + (i).ToString());
            }
            
        }

        private void fillDropSetLists(string itemName)
        {
            int position = cbDrop1.Items.Count;
            cbDrop1.Items.Insert(position, itemName);
            cbDrop2.Items.Insert(position, itemName);
            cbDrop3.Items.Insert(position, itemName);
            cbDrop4.Items.Insert(position, itemName);
            cbDrop5.Items.Insert(position, itemName);
            cbDrop6.Items.Insert(position, itemName);
            cbDrop7.Items.Insert(position, itemName);
            cbDrop8.Items.Insert(position, itemName);
            cbDrop9.Items.Insert(position, itemName);
            cbDrop10.Items.Insert(position, itemName);
            cbDrop11.Items.Insert(position, itemName);
            cbDrop12.Items.Insert(position, itemName);
            cbDrop13.Items.Insert(position, itemName);
            cbDrop14.Items.Insert(position, itemName);
            cbDrop15.Items.Insert(position, itemName);
            cbDrop16.Items.Insert(position, itemName);
        }

        private void loadDropSetFields()
        {
            int dropSetIndex = cbDrops.SelectedIndex;
            cbDrop1.SelectedIndex = this.romData.dropSets[dropSetIndex, 0];
            cbDrop2.SelectedIndex = this.romData.dropSets[dropSetIndex, 1];
            cbDrop3.SelectedIndex = this.romData.dropSets[dropSetIndex, 2];
            cbDrop4.SelectedIndex = this.romData.dropSets[dropSetIndex, 3];
            cbDrop5.SelectedIndex = this.romData.dropSets[dropSetIndex, 4];
            cbDrop6.SelectedIndex = this.romData.dropSets[dropSetIndex, 5];
            cbDrop7.SelectedIndex = this.romData.dropSets[dropSetIndex, 6];
            cbDrop8.SelectedIndex = this.romData.dropSets[dropSetIndex, 7];
            cbDrop9.SelectedIndex = this.romData.dropSets[dropSetIndex, 8];
            cbDrop10.SelectedIndex = this.romData.dropSets[dropSetIndex, 9];
            cbDrop11.SelectedIndex = this.romData.dropSets[dropSetIndex, 10];
            cbDrop12.SelectedIndex = this.romData.dropSets[dropSetIndex, 11];
            cbDrop13.SelectedIndex = this.romData.dropSets[dropSetIndex, 12];
            cbDrop14.SelectedIndex = this.romData.dropSets[dropSetIndex, 13];
            cbDrop15.SelectedIndex = this.romData.dropSets[dropSetIndex, 14];
            cbDrop16.SelectedIndex = this.romData.dropSets[dropSetIndex, 15];
        }

        private void saveDropSetFields()
        {
            int dropSetIndex = previousDropSetIndex;
            this.romData.dropSets[dropSetIndex, 0] = cbDrop1.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 1] = cbDrop2.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 2] = cbDrop3.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 3] = cbDrop4.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 4] = cbDrop5.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 5] = cbDrop6.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 6] = cbDrop7.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 7] = cbDrop8.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 8] = cbDrop9.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 9] = cbDrop10.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 10] = cbDrop11.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 11] = cbDrop12.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 12] = cbDrop13.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 13] = cbDrop14.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 14] = cbDrop15.SelectedIndex;
            this.romData.dropSets[dropSetIndex, 15] = cbDrop16.SelectedIndex;
        }

        private void cbDrops_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!justLoadedRom)
            {
                saveDropSetFields();
            }
            loadDropSetFields();
            previousDropSetIndex = this.cbDrops.SelectedIndex;
        }

        private void fillUpExpTableList()
        {
            for (var i = 0; i < Rom.maxExperienceTable; i++)
            {
                cbExpTableLevel.Items.Insert(i, (i + 1).ToString());
            }
        }

        private void loadExperienceTableFields()
        {
            int experienceTableIndex = cbExpTableLevel.SelectedIndex;
            numExpTable.Value = this.romData.experienceTable[experienceTableIndex];
        }

        private void saveExperienceTableFields()
        {
            int experienceTableIndex = previousExperienceTableIndex;
            this.romData.experienceTable[experienceTableIndex] = Convert.ToInt32(numExpTable.Value);
        }

        private void cbExpTableLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!justLoadedRom)
            {
                saveExperienceTableFields();
            }
            loadExperienceTableFields();
            previousExperienceTableIndex = this.cbExpTableLevel.SelectedIndex;

        }



        /*************************************************************
         * ***********************************************************
         * ***********************************************************
         * 
         * GENERIC STUFF
         * 
         * ***********************************************************
         * ***********************************************************
         * ***********************************************************/

        /*****************************************************
         * 
         * saveToolStripMenuItem_Click
         * 
        *****************************************************/

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveCharacterFields();
            saveSpellFields();
            saveMonsterFields();
            saveWeaponFields();
            saveArmorFields();
            saveDropSetFields();
            saveExperienceTableFields();
            this.romData.SaveData(selectedRom);
            MessageBox.Show("Saved!");
        }

        private string hex2bin(string hexstring, int bytes)
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutWindow = new AboutBox1();
            aboutWindow.Show();
        }
    }
}
