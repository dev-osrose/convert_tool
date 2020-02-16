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
  public class ItemData
  {
    public enum ItemType : int {
      FACE = 1,
      CAP,
      BODY,
      ARMS,
      FOOT,
      BACK,
      JEWEL,
      WEAPON,
      SUB_WEAPON,
      USEITEM,
      JEMITEM,
      NATURAL,
      QUESTITEM,
      PAT,
      MAX_ITEM_TYPES
    };

    public void Load(ItemType type = 0, string stbPath = null, string stlPath = null)
    {
      if (type == 0 || stbPath == null || stlPath == null)
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
      for (var i = 0; i < dataFile.RowCount; i++)
      {
        StringTableRow strTableRow;
        var curRow = dataFile[i];
        try
        {
          strTableRow = stringFile[curRow[(dataFile.ColumnCount - 1)]];
        }
        catch (ArgumentException)
        {
          continue;
        }

        var itemName = strTableRow.GetText();
        var itemDesc = strTableRow.GetDescription();

        if(itemName.Length == 0 || itemDesc.Length == 0) continue;

        string script = "";
        List<string> reqTable = new List<string>();
        List<string> bonusTable = new List<string>();
        double priceSell = 0.0f;
        int subtype, priceBuy, weight, attack, defense, range, slots, equipJobs, groundViewModel, durability, attackSpeed, magic, moveSpeed, usageRestrictions;
        subtype = priceBuy = weight = attack = defense = range = slots = equipJobs = groundViewModel = durability = attackSpeed = magic = moveSpeed = usageRestrictions = 0;
        
        int.TryParse(curRow[4], out usageRestrictions);
        int.TryParse(curRow[5], out subtype);
        int.TryParse(curRow[6], out priceBuy);
        double.TryParse(curRow[7], out priceSell);
        int.TryParse(curRow[8], out weight);

        int.TryParse(curRow[11], out groundViewModel);
        int.TryParse(curRow[17], out equipJobs);

        try
        {
          for (var reqIndex = 0; reqIndex < 2; reqIndex++)
          {
            var reqType = curRow[(20 + (reqIndex * 2))];
            var value = curRow[(21 + (reqIndex * 2))];

            if (reqType.Length == 0 || value.Length == 0) continue;

            var reqId = "reqTable[" + (reqIndex + 1) + "]";
            reqTable.Add(reqId + " = {}\n" +
                         reqId + ".type = " + reqType + "\n" +
                         reqId + ".value = " + value + "\n");
          }

          for (var bonusIndex = 0; bonusIndex < 2; bonusIndex++)
          {
            var bonusType = curRow[(25 + (bonusIndex * 3))];
            var value = curRow[(26 + (bonusIndex * 3))];

            if (bonusType.Length == 0 || value.Length == 0) continue;

            var bonusId = "bonusTable[" + (bonusIndex + 1) + "]";
            bonusTable.Add(bonusId + " = {}\n" +
                         bonusId + ".type = " + bonusType + "\n" +
                         bonusId + ".value = " + value + "\n");
          }

          int.TryParse(curRow[30], out durability);
          int.TryParse(curRow[32], out defense);

          switch (type)
          {
            case ItemType.BACK:
            case ItemType.FOOT:
            {
              int.TryParse(curRow[34], out moveSpeed);
              break;
            }
            case ItemType.WEAPON: 
            {
              int.TryParse(curRow[31], out slots);
              int.TryParse(curRow[34], out range);
              int.TryParse(curRow[36], out attack);
              int.TryParse(curRow[37], out attackSpeed);
              int.TryParse(curRow[38], out magic);
              break;
            }
            case ItemType.SUB_WEAPON: 
            {
              int.TryParse(curRow[31], out slots);
              break;
            }
            default:
              break;
          }
        }
        catch (ArgumentOutOfRangeException e)
        {
          //Console.WriteLine(e);
          //Shit borked..... naaaa just this STB prob doesn't have extra data in it
        }

#if OUTPUT_STB_DATA_COMMENT
        script += "--[[ ";
        for (var j = 0; j < dataFile.ColumnCount - 1; j++)
        {
          script += "COLUMN " + j+1 + " =='" + curRow[j+1] + "'\n";
        }
        script += "--]]\n\n";
#endif

        if (reqTable.Count > 0)
        {
          script += "reqTable = {}\n";
        }

        foreach (var requirement in reqTable)
        {
          script += requirement;
        }


        if (bonusTable.Count > 0)
        {
          script += "bonusTable = {}\n";
        }
        foreach (var bonus in bonusTable)
        {
          script += bonus;
        }

        script +=
          @"
function OnInit()
  return true
end

function OnCreate()
  return true
end

function OnDelete()
  return true
end

function OnEquip(entity)";
        if (reqTable.Count > 0)
        {
          script +=
          @"
  for i, data in ipairs(reqTable) do
    if data.value > getAttr(entity, data.type) then
      return false
    end
  end";
        }

        if (bonusTable.Count > 0)
        {
          script +=
          @"
  for i, data in ipairs(bonusTable) do
    addBonusAttr(entity, data.type, data.value)
  end";
        }

        script +=
          @"
  return true
end

function OnUnequip(entity)";

        if (bonusTable.Count > 0)
        {
          script +=
            @"
  for i, data in ipairs(bonusTable) do
    removeBonusAttr(entity, data.type, data.value)
  end";
        }

                script +=
                  @"
  return true
end

function OnDrop(entity)
  return true
end

function OnPickup(entity)
  return true
end

function OnUse(entity)
  return true
end

function GetAttackSpd()
  return " + attackSpeed + @"
end

function GetMoveSpd()
  return " + moveSpeed + @"
end

function GetMagic()
  return " + magic + @"
end

function GetUsageRestrictions()
  return " + usageRestrictions + @"
end";

        itemDesc = itemDesc.Replace("\"", "\\\"");
        string sqlEntry =
          "REPLACE into item_db(id, name, `desc`, type, subtype, price_buy, price_sell, weight, attack, defense, `range`, slots, equip_jobs, view_id, script) ";
        sqlEntry += "values(" + i + ", \"" + itemName + "\", \"" + itemDesc + "\", " + (int)(type) + ", " + subtype + ", " + priceBuy + ", " + priceSell + ", " + weight + ", " + attack + ", " + defense + ", " + range.ToString("G", CultureInfo.InvariantCulture) + ", " + slots + ", " + equipJobs + ", " + groundViewModel + ", \"" + script + "\");";

        sqlFileList.Add(sqlEntry);
      }

      var sqlFile = new System.IO.StreamWriter("srv_data\\item_db.sql", true);
      using (sqlFile)
      {
        foreach (var itemLine in sqlFileList)
        {
          sqlFile.WriteLine(itemLine);
        }
      }
    }
  }
}
