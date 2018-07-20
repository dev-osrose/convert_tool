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

        string sqlEntry =
          "REPLACE into mob_db(id, name, `desc`, type, subtype, price_buy, price_sell, weight, attack, defense, `range`, slots, equip_jobs, view_id, script) ";
        sqlEntry += "values(" + i + ", \"" + itemName + "\", \"" + itemDesc + "\", " + (int)(type) + ", " + subtype + ", " + priceBuy + ", " + priceSell + ", " + weight + ", " + attack + ", " + defense + ", " + range + ", " + slots + ", " + equipJobs + ", " + groundViewModel + ", \"" + script + "\");";

        sqlFileList.Add(sqlEntry);
      }

      var sqlFile = new System.IO.StreamWriter("srv_data\\mob_db.sql", true);
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