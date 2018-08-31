#region License

/**
 * Copyright (C) 2018 Chirstopher Torres (Raven)
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

#endregion

//#define OUTPUT_STB_DATA_COMMENT
using System;
using System.Collections.Generic;
using System.IO;
using Revise.Files.STB;
using Revise.Files.STL;
using SharpDX;

namespace convert_tool
{
  public class MobData
  {
    public void Load(string stbPath = null, string stlPath = null)
    {
      if (stbPath == null || stlPath == null)
        return;

      var stringFile = new StringTableFile();
      var dataFile = new DataFile();

      Console.Write("Attempting to load \"" + stbPath + "\" and \"" + stlPath + "\" - ");

      try
      {
        dataFile.Load(stbPath);
        stringFile.Load(stlPath);
      }
      catch (FileNotFoundException e)
      {
        Console.Write("Failed!\n");
        Console.WriteLine(e);
        return;
      }

      Console.Write("Successful!\n");
      List<string> sqlFileList = new List<string>();
      List<string> luaFileList = new List<string>();
      for (var i = 0; i < dataFile.RowCount; i++)
      {
        StringTableRow strTableRow;
        var curRow = dataFile[i];
        try
        {
          strTableRow = stringFile[curRow[40]];
        }
        catch (ArgumentException)
        {
          continue;
        }

        var npcName = strTableRow.GetText();
        var npcDesc = curRow[41];

        if (npcName.Length == 0 || npcDesc.Length == 0) continue;

        int npcHeight,
          npcWalkSpeed,
          npcRunSpeed,
          npcScale,
          npcRWeapon,
          npcLWeaponm,
          npcLevel,
          npcHp,
          npcAttack,
          npcHit,
          npcDef,
          npcRes,
          npcAvoid,
          npcAttackSpd,
          npcIsMagicDamage,
          npcAiType,
          npcGiveExp,
          npcDropType,
          npcMarkNumber,
          npcDropMoney,
          npcDropItem,
          npcUnionNumber,
          npcNeedSummonCount,
          npcSellTab0,
          npcSellTab1,
          npcSellTab2,
          npcSellTab3,
          npcCanTarget,
          npcAttackRange,
          npcType,
          npcHitMaterialType,
          npcFaceIcon,
          npcSummonMobType,
          npcNormalEffectSound,
          npcAttackSound,
          npcHitSound,
          npcHandHitEffect,
          npcDeadEffect,
          npcDieSound,
          npcQuestType,
          npcGlowColor,
          npcCreateEffect,
          npcCreateSound;

        npcHeight = npcWalkSpeed = npcRunSpeed = npcScale = npcRWeapon = npcLWeaponm = npcLevel = npcHp = npcAttack =
          npcHit = npcDef = npcRes = npcAvoid = npcAttackSpd = npcIsMagicDamage =
            npcAiType = npcGiveExp = npcDropType = npcMarkNumber = npcDropMoney = npcDropItem = npcUnionNumber =
              npcNeedSummonCount =
                npcSellTab0 = npcSellTab1 = npcSellTab2 = npcSellTab3 = npcCanTarget = npcAttackRange = npcType =
                  npcHitMaterialType = npcFaceIcon = npcSummonMobType = npcNormalEffectSound = npcAttackSound =
                    npcHitSound = npcHandHitEffect = npcDeadEffect =
                      npcDieSound = npcQuestType = npcGlowColor = npcCreateEffect = npcCreateSound = 0;


        int.TryParse(curRow[02], out npcWalkSpeed); //lua?
        int.TryParse(curRow[03], out npcRunSpeed); //lua?
        int.TryParse(curRow[04], out npcScale); //lua
        int.TryParse(curRow[05], out npcRWeapon); //lua?
        int.TryParse(curRow[06], out npcLWeaponm); //lua?
        int.TryParse(curRow[07], out npcLevel); //lua?
        int.TryParse(curRow[08], out npcHp); //sql? lua?
        int.TryParse(curRow[09], out npcAttack); //lua?
        int.TryParse(curRow[10], out npcHit); //lua?
        int.TryParse(curRow[11], out npcDef); //lua?
        int.TryParse(curRow[12], out npcRes); //lua?
        int.TryParse(curRow[13], out npcAvoid); //lua?
        int.TryParse(curRow[14], out npcAttackSpd); //lua?
        int.TryParse(curRow[15], out npcIsMagicDamage); //lua?
        int.TryParse(curRow[16], out npcAiType); //lua
        int.TryParse(curRow[17], out npcGiveExp); //lua
        int.TryParse(curRow[18], out npcDropType); //lua
        int.TryParse(curRow[18], out npcMarkNumber); // NPC icon (only used with EVO style npc boxes)
        int.TryParse(curRow[19], out npcDropMoney); //lua

        int.TryParse(curRow[20], out npcDropItem); //lua
        int.TryParse(curRow[20], out npcUnionNumber); //sql
        int.TryParse(curRow[21], out npcNeedSummonCount); //lua, used for unknown...
        int.TryParse(curRow[21], out npcSellTab0); //lua
        int.TryParse(curRow[22], out npcSellTab1); //lua
        int.TryParse(curRow[23], out npcSellTab2); //lua 
        int.TryParse(curRow[24], out npcSellTab3); //lua
        int.TryParse(curRow[25], out npcCanTarget);
        int.TryParse(curRow[26], out npcAttackRange); //lua?
        int.TryParse(curRow[27], out npcType); //sql?
        int.TryParse(curRow[28], out npcHitMaterialType);
        int.TryParse(curRow[29], out npcFaceIcon); // NPC face icon (only used with EVO style npc boxes)
        int.TryParse(curRow[29], out npcSummonMobType);

        int.TryParse(curRow[30], out npcNormalEffectSound);
        int.TryParse(curRow[31], out npcAttackSound);
        int.TryParse(curRow[32], out npcHitSound);
        int.TryParse(curRow[33], out npcHandHitEffect);
        int.TryParse(curRow[34], out npcDeadEffect);
        int.TryParse(curRow[35], out npcDieSound);
        int.TryParse(curRow[38], out npcQuestType); //lua
        int.TryParse(curRow[39], out npcGlowColor);

        int.TryParse(curRow[42], out npcHeight);
        int.TryParse(curRow[44], out npcCreateEffect);
        int.TryParse(curRow[45], out npcCreateSound);


        string sqlEntry =
          "REPLACE into mob_db(id, name, `desc`, subtype, price_buy, price_sell, weight, attack, defense, `range`, slots, equip_jobs, view_id, script) ";
/*
         sqlEntry += "values(" + i + ", \"" + npcName + "\", \"" + npcDesc + ", " + subtype + ", " + priceBuy + ", " +
                    priceSell + ", " + weight + ", " + attack + ", " + defense + ", " + range + ", " + slots + ", " +
                    equipJobs + ", " + groundViewModel + ", \"" + script + "\");";
*/

        sqlFileList.Add(sqlEntry);
      }

      /*var sqlFile = new System.IO.StreamWriter("srv_data\\mob_db.sql", true);
      using (sqlFile)
      {
        foreach (var mobLine in sqlFileList)
        {
          sqlFile.WriteLine(mobLine);
        }
      }*/

      /*var luaFile = new System.IO.StreamWriter("srv_data\\scripts\\npc\\test.lua", true);
      using (luaFile)
      {
        foreach (var mobLine in luaFileList)
        {
          luaFile.WriteLine(mobLine);
        }
      }*/
    }
  }
}