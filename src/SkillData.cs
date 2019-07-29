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
  public class SkillData
  {
    public enum SkillType : int {
      INVALID_SKILL = 0,
      PASSIVE_SKILL = 15,
      MAX_SKILL_TYPES = 21
    };

    public enum TargetFilter : int
    {
      SELF = 0,
      GROUP,
      GUILD,
      ALL_FRIENDLY,
      MOB,
      ALL_ENEMIES,
      PC_ENEMIES,
      ALL_PCs,
      ALL_CHARACTERS,
      DEAD_USERS,
      ENEMY_MOB,
      MAX_TARGET_TYPES
    };

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

        var skillName = strTableRow.GetText();
        var skillDesc = strTableRow.GetDescription();

        if(skillName.Length == 0 || skillDesc.Length == 0) continue;

        string script = "";
        int level, type, range, class_, power, pointsToLevelUp, zulyNeededToLevelUp, usageAttribute;
        level = type = range = class_ = power = pointsToLevelUp = zulyNeededToLevelUp = usageAttribute = 0;


//enum
//{
//  SKILL_TYPE_01 = 1,
//  SKILL_TYPE_02,
//  SKILL_TYPE_03,
//  SKILL_TYPE_04,
//  SKILL_TYPE_05,
//  SKILL_TYPE_06,
//  SKILL_TYPE_07,
//  SKILL_TYPE_08,
//  SKILL_TYPE_09,
//  SKILL_TYPE_10,
//  SKILL_TYPE_11,
//  SKILL_TYPE_12,
//  SKILL_TYPE_13,
//  SKILL_TYPE_14,
//  SKILL_TYPE_15,
//  // ÆÐ½Ãºê ½ºÅ³
//  SKILL_TYPE_PASSIVE = SKILL_TYPE_15,
//  SKILL_TYPE_16,
//  // Emotion
//  SKILL_TYPE_17,
//  // Self & damage
//  SKILL_TYPE_18,
//  // warp skill
//  SKILL_TYPE_19,
//  SKILL_TYPE_20,
//};

//SKILL_1LEV_INDEX(S)					      [ S ][  1 ]
//SKILL_LEVEL(S)						        [ S ][  2 ]
//SKILL_NEED_LEVELUPPOINT(S)	      [ S ][  3 ]
//SKILL_TAB_TYPE(S)					        [ S ][  4 ]
//SKILL_TYPE( I )						        [ I ][  5 ]
//SKILL_DISTANCE( I )					      [ I ][  6 ]
//SKILL_WARP_PLANET_NO( I )		      [ I ][  6 ]
//SKILL_CLASS_FILTER( I )			      [ I ][  7 ]
//SKILL_SCOPE(S)						        [ S ][  8 ]
//SKILL_POWER(S)						        [ S ][  9 ]
//SKILL_ITEM_MAKE_NUM		            SKILL_POWER
//SKILL_HARM( I )						        [ I ][ 10 ]
//SKILL_STATE_STB( I,C )			      [ I ][ 11+C ]
//SKILL_STATE_STB1( I )				      [ I ][ 11 ]
//SKILL_STATE_STB2( I )				      [ I ][ 12 ]
//SKILL_SUCCESS_RATIO( I )		      [ I ][ 13 ]		
//SKILL_DURATION( I )					      [ I ][ 14 ]
//SKILL_DAMAGE_TYPE(S)				      [ S ][ 15 ]
//SKILL_USE_PROPERTY_CNT			      2
//  SKILL_USE_PROPERTY(S,T)			      [ S ][ 16+(T)*2 ]
//  SKILL_USE_VALUE(S,T)				      [ S ][ 17+(T)*2 ]
//SKILL_RELOAD_TIME(S)				      [ S ][ 20 ]
//SKILL_INCREASE_ABILITY_CNT			  2
//  SKILL_INCREASE_ABILITY(S,T)			  [ S ][ 21+(T)*3 ]
//  SKILL_INCREASE_ABILITY_VALUE(S,T)	[ S ][ 22+(T)*3 ]
//SKILL_CHANGE_ABILITY_RATE(S,T)		[ S ][ 23+(T)*3 ]
//SKILL_WARP_ZONE_NO( S )				    [ S ][ 21 ]
//SKILL_WARP_ZONE_XPOS( S )			    [ S ][ 22 ]
//SKILL_WARP_ZONE_YPOS( S )			    [ S ][ 23 ]
//SKILL_RELOAD_TYPE(S)				      [ S ][ 27 ]
//SKILL_SUMMON_PET(S)					      [ S ][ 28 ]
//SKILL_ACTION_MODE(S)				      [ S ][ 29 ]
//SKILL_NEED_WEAPON_CNT				      5
//  SKILL_NEED_WEAPON(S,T)				  [ S ][ 30+(T) ]
//SKILL_AVAILBLE_CLASS_SET(S)		    [ S ][ 35 ]
//SKILL_AVAILBLE_UNION_CNT			    3
//  SKILL_AVAILBLE_UNION(S,T)			  [ S ][ 36+(T) ]
//SKILL_NEED_SKILL_CNT				      3
//  SKILL_NEED_SKILL_INDEX(S,T)		  [ S ][ 39+(T)*2 ]
//  SKILL_NEDD_SKILL_LEVEL(S,T)		  [ S ][ 40+(T)*2 ]
//SKILL_NEED_ABILITY_TYPE_CNT		    2
//  SKILL_NEED_ABILITY_TYPE(S,T)	  [ S ][ 45+(T)*2 ]
//  SKILL_NEED_ABILITY_VALUE(S,T)	  [ S ][ 46+(T)*2 ]
//SKILL_SCRIPT1( I )					      [ I ][ 49 ]
//SKILL_RESERVE_02( I )				      [ I ][ 50 ]
//SKILL_LEVELUP_NEED_ZULY( I )		  [ I ][ 85 ]
//SKILL_ATTRIBUTE( I )				      [ I ][ 86 ]
//SKILL_ATTRIBUTE_AVATAR				1
//SKILL_ATTRIBUTE_CART				  2
//SKILL_ATTRIBUTE_CASTLEGEAR		4

//-----------------------------------
//Graphics related
//SKILL_ICON_NO( I )					      [ I ][ 51 ]
//SKILL_ANI_CASTING(S)				      [ S ][ 52 ]
//SKILL_ANI_CASTING_SPEED(S)		    [ S ][ 53 ]
//SKILL_ANI_CASTING_REPEAT(S)		    [ S ][ 54 ]
//SKILL_ANI_CASTING_REPEAT_CNT(S)		[ S ][ 55 ]
//SKILL_CASTING_EFFECT_CNT			    4
//  SKILL_CASTING_EFFECT( I,T )			  [ I ][ 56 + (T * 3) ]
//  SKILL_CASTING_EFFECT_POINT( I,T )	[ I ][ 57 + (T * 3) ]
//  SKILL_CASTING_SOUND( I,T )			  [ I ][ 58 + (T * 3) ]
//SKILL_ANI_ACTION_TYPE(S)			    [ S ][ 68 ]
//SKILL_ANI_ACTION_SPEED(S)			    [ S ][ 69 ]
//SKILL_ANI_HIT_COUNT(S)				    [ S ][ 70 ]
//SKILL_BULLET_NO( I )				      [ I ][ 71 ]
//SKILL_BULLET_LINKED_POINT( I )		[ I ][ 72 ]
//SKILL_BULLET_FIRE_SOUND( I )		  [ I ][ 73 ]
//SKILL_HIT_EFFECT( I )				      [ I ][ 74 ]
//SKILL_HIT_EFFECT_LINKED_POINT( I )	[ I ][ 75 ]
//SKILL_HIT_SOUND( I )				        [ I ][ 76 ]
//SKILL_HIT_DUMMY_EFFECT_CNT			    2
//  SKILL_HIT_DUMMY_EFFECT( I, T )			          [ I ][ 77 + 3*T ]
//  SKILL_HIT_DUMMY_EFFECT_LINKED_POINT( I, T )		[ I ][ 78 + 3*T ]
//  SKILL_HIT_DUMMY_SOUND( I, T )					        [ I ][ 79 + 3*T ]
//SKILL_AREA_HIT_EFFECT( I )			  [ I ][ 83 ]
//SKILL_AREA_HIT_SOUND( I )			    [ I ][ 84 ]

        int.TryParse(curRow[3], out level);
        int.TryParse(curRow[4], out pointsToLevelUp);
        int.TryParse(curRow[6], out type);
        int.TryParse(curRow[7], out range);
        int.TryParse(curRow[8], out class_);
        int.TryParse(curRow[10], out power);
        script = curRow[50]; // not used by any skill
        int.TryParse(curRow[86], out zulyNeededToLevelUp);
        int.TryParse(curRow[87], out usageAttribute);

//#if OUTPUT_STB_DATA_COMMENT
//        script += "--[[ ";
//        for (var j = 0; j < dataFile.ColumnCount - 1; j++)
//        {
//          script += "COLUMN " + j+1 + " =='" + curRow[j+1] + "'\n";
//        }
//        script += "--]]\n\n";
//#endif

        skillDesc = skillDesc.Replace("\"", "\\\"");
        string sqlEntry =
          "REPLACE into skill_db(id, name, `desc`, level, type, `range`, class_, power, script) ";
        sqlEntry += "values(" + i + ", \"" + skillName + "\", \"" + skillDesc + "\", " + (int)(level) + ", " + (int)(type) + ", " + range + ", " + class_ + ", " + power + ", \"" + script + "\");";

        sqlFileList.Add(sqlEntry);
      }

      var sqlFile = new System.IO.StreamWriter("srv_data\\skill_db.sql", true);
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