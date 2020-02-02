﻿/*
    ------------------------------------------------------------------------------------
    LICENSE:
    ------------------------------------------------------------------------------------
    This file is part of EVE#: The EVE Online Server Emulator
    Copyright 2012 - Glint Development Group
    ------------------------------------------------------------------------------------
    This program is free software; you can redistribute it and/or modify it under
    the terms of the GNU Lesser General Public License as published by the Free Software
    Foundation; either version 2 of the License, or (at your option) any later
    version.

    This program is distributed in the hope that it will be useful, but WITHOUT
    ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
    FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License along with
    this program; if not, write to the Free Software Foundation, Inc., 59 Temple
    Place - Suite 330, Boston, MA 02111-1307, USA, or go to
    http://www.gnu.org/copyleft/lesser.txt.
    ------------------------------------------------------------------------------------
    Creator: Almamu
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marshal;
using Common;
using Common.Database;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Common.Packets;
using Common.Utils;
using Node.Database;

namespace Node
{
    public class CacheStorage : DatabaseAccessor
    {
	    public enum CacheObjectType
	    {
		    TupleSet = 0,
		    Rowset = 1,
		    CRowset = 2,
		    PackedRowList = 3
	    };
	    
        public static string[] LoginCacheTable =
        {
        	"config.BulkData.ramactivities",
	        "config.BulkData.billtypes",
	        "config.Bloodlines",
	        "config.Units",
	        "config.BulkData.tickernames",
	        "config.BulkData.ramtyperequirements",
	        "config.BulkData.ramaltypesdetailpergroup",
	        "config.BulkData.ramaltypes",
	        "config.BulkData.allianceshortnames",
	        "config.BulkData.ramcompletedstatuses",
	        "config.BulkData.categories",
	        "config.BulkData.invtypereactions",
	        "config.BulkData.dgmtypeeffects",
	        "config.BulkData.metagroups",
	        "config.BulkData.ramtypematerials",
	        "config.BulkData.ramaltypesdetailpercategory",
	        "config.BulkData.owners",
	        "config.StaticOwners",
	        "config.Races",
	        "config.Attributes",
	        "config.BulkData.dgmtypeattribs",
	        "config.BulkData.locations",
	        "config.BulkData.locationwormholeclasses",
	        "config.BulkData.groups",
	        "config.BulkData.shiptypes",
	        "config.BulkData.dgmattribs",
	        "config.Flags",
	        "config.BulkData.bptypes",
	        "config.BulkData.graphics",
	        "config.BulkData.mapcelestialdescriptions",
	        "config.BulkData.certificates",
	        "config.StaticLocations",
	        "config.InvContrabandTypes",
	        "config.BulkData.certificaterelationships",
	        "config.BulkData.units",
	        "config.BulkData.dgmeffects",
	        "config.BulkData.types",
	        "config.BulkData.invmetatypes"
        };

        public static string[] LoginCacheQueries =
        {
            "SELECT activityID, activityName, iconNo, description, published FROM ramActivities",
            "SELECT billTypeID, billTypeName, description FROM billTypes",
            "SELECT bloodlineID, bloodlineName, raceID, description, maleDescription, femaleDescription, shipTypeID, corporationID, perception, willpower, charisma, memory, intelligence, graphicID, shortDescription, shortMaleDescription, shortFemaleDescription, 0 AS dataID FROM chrBloodlines",
            "SELECT unitID, unitName, displayName FROM eveUnits",
            "SELECT corporationID, tickerName, shape1, shape2, shape3, color1, color2, color3 FROM corporation WHERE hasPlayerPersonnelManager = 0",
            "SELECT typeID, activityID, requiredTypeID, quantity, damagePerJob, recycle FROM typeActivityMaterials WHERE damagePerJob != 1.0 OR recycle = 1",
            "SELECT a.assemblyLineTypeID, b.activityID, a.groupID, a.timeMultiplier, a.materialMultiplier FROM ramAssemblyLineTypeDetailPerGroup AS a LEFT JOIN ramAssemblyLineTypes AS b ON a.assemblyLineTypeID = b.assemblyLineTypeID",
            "SELECT assemblyLineTypeID, assemblyLineTypeName, assemblyLineTypeName AS typeName, description, activityID, baseTimeMultiplier, baseMaterialMultiplier, volume FROM ramAssemblyLineTypes",
            "SELECT allianceID, shortName FROM alliance_shortnames",
            "SELECT completedStatusID, completedStatusName, completedStatusText FROM ramCompletedStatuses",
            "SELECT categoryID, categoryName, description, graphicID, published, 0 AS dataID FROM invCategories",
            "SELECT reactionTypeID, input, typeID, quantity FROM invTypeReactions",
            "SELECT typeID, effectID, isDefault FROM dgmTypeEffects",
            "SELECT metaGroupID, metaGroupName, description, graphicID, 0 AS dataID FROM invMetaGroups",
            "SELECT typeID, requiredTypeID AS materialTypeID, quantity FROM typeActivityMaterials WHERE activityID = 6 AND damagePerJob = 1.0 UNION SELECT productTypeID AS typeID, requiredTypeID AS materialTypeID, quantity FROM typeActivityMaterials JOIN invBlueprintTypes ON typeID = blueprintTypeID WHERE activityID = 1 AND damagePerJob = 1.0",
            "SELECT a.assemblyLineTypeID, b.activityID, a.categoryID, a.timeMultiplier, a.materialMultiplier FROM ramAssemblyLineTypeDetailPerCategory AS a LEFT JOIN ramAssemblyLineTypes AS b ON a.assemblyLineTypeID = b.assemblyLineTypeID",
            "SELECT cacheOwner AS ownerID, cacheOwnerName AS ownerName, cacheOwnerType AS typeID FROM usercache",
            "SELECT ownerID, ownerName, typeID FROM eveStaticOwners",
            "SELECT raceID, raceName, description, graphicID, shortDescription, 0 AS dataID FROM chrRaces",
            "SELECT attributeID, attributeName, description, graphicID FROM chrAttributes",
            "SELECT	dgmTypeAttributes.typeID,	dgmTypeAttributes.attributeID,	IF(valueInt IS NULL, valueFloat, valueInt) AS value FROM dgmTypeAttributes",
            "SELECT locationID, locationName, x, y, z FROM cacheLocations",
            "SELECT locationID, wormholeClassID FROM mapLocationWormholeClasses",
            "SELECT groupID, categoryID, groupName, description, graphicID, useBasePrice, allowManufacture, allowRecycler, anchored, anchorable, fittableNonSingleton, 1 AS published, 0 AS dataID FROM invGroups",
            "SELECT shipTypeID,weaponTypeID,miningTypeID,skillTypeID FROM invShipTypes",
            "SELECT attributeID, attributeName, attributeCategory, description, maxAttributeID, attributeIdx, graphicID, chargeRechargeTimeID, defaultValue, published, displayName, unitID, stackable, highIsGood, categoryID, 0 AS dataID FROM dgmAttributeTypes",
            "SELECT flagID, flagName, flagText, flagType, orderID FROM invFlags",
            "SELECT invTypes.typeName AS blueprintTypeName, invTypes.description, invTypes.graphicID, invTypes.basePrice, blueprintTypeID, parentBlueprintTypeID, productTypeID, productionTime, techLevel, researchProductivityTime, researchMaterialTime, researchCopyTime, researchTechTime, productivityModifier, materialModifier, wasteFactor, chanceOfReverseEngineering, maxProductionLimit FROM invBlueprintTypes, invTypes WHERE invBlueprintTypes.blueprintTypeID = invTypes.typeID",
            "SELECT graphicID, url3D, urlWeb, icon, urlSound, explosionID FROM eveGraphics",
            "SELECT celestialID, description FROM mapCelestialDescriptions",
            "SELECT certificateID, categoryID, classID, grade, iconID, corpID, description, 0 AS dataID FROM crtCertificates",
            "SELECT locationID, locationName, x, y, z FROM eveStaticLocations",
            "SELECT factionID, typeID, standingLoss, confiscateMinSec, fineByValue, attackMinSec FROM invContrabandTypes",
            "SELECT relationshipID, parentID, parentTypeID, parentLevel, childID, childTypeID FROM crtRelationships",
            "SELECT unitID,unitName,displayName FROM eveUnits",
            "SELECT effectID, effectName, effectCategory, preExpression, postExpression, description, guid, graphicID, isOffensive, isAssistance, durationAttributeID, trackingSpeedAttributeID, dischargeAttributeID, rangeAttributeID, falloffAttributeID, published, displayName, isWarpSafe, rangeChance, electronicChance, propulsionChance, distribution, sfxName, npcUsageChanceAttributeID, npcActivationChanceAttributeID, 0 AS fittingUsageChanceAttributeID, 0 AS dataID FROM dgmEffects",
            "SELECT typeID, groupID, typeName, description, graphicID, radius, mass, volume, capacity, portionSize, raceID, basePrice, published, marketGroupID, chanceOfDuplicating, 0 AS dataID FROM invTypes",
            "SELECT typeID, parentTypeID, metaGroupID FROM invMetaTypes"
        };

        public static CacheObjectType[] LoginCacheTypes =
        {
	        CacheObjectType.TupleSet,
	        CacheObjectType.TupleSet,
	        CacheObjectType.Rowset,
	        CacheObjectType.Rowset,
	        CacheObjectType.TupleSet,
	        CacheObjectType.Rowset,
	        CacheObjectType.CRowset,
	        CacheObjectType.CRowset,
	        CacheObjectType.TupleSet,
	        CacheObjectType.TupleSet,
	        CacheObjectType.TupleSet,
	        CacheObjectType.CRowset,
	        CacheObjectType.Rowset,
	        CacheObjectType.TupleSet,
	        CacheObjectType.Rowset,
	        CacheObjectType.CRowset,
	        CacheObjectType.TupleSet,
	        CacheObjectType.Rowset,
	        CacheObjectType.Rowset,
	        CacheObjectType.Rowset,
	        CacheObjectType.PackedRowList,
	        CacheObjectType.TupleSet,
	        CacheObjectType.CRowset,
	        CacheObjectType.TupleSet,
	        CacheObjectType.CRowset,
	        CacheObjectType.TupleSet,
	        CacheObjectType.Rowset,
	        CacheObjectType.TupleSet,
	        CacheObjectType.TupleSet,
	        CacheObjectType.TupleSet,
	        CacheObjectType.CRowset,
	        CacheObjectType.Rowset,
	        CacheObjectType.Rowset,
	        CacheObjectType.CRowset,
	        CacheObjectType.TupleSet,
	        CacheObjectType.TupleSet,
	        CacheObjectType.TupleSet,
	        CacheObjectType.CRowset
        };
/*
        public static string[] CreateCharacterCacheTable = new string[] 
        {
          	"charCreationInfo.bl_eyebrows",
	        "charCreationInfo.bl_eyes",
	        "charCreationInfo.bl_decos",
	        "charCreationInfo.bloodlines",
	        "charCreationInfo.bl_hairs",
	        "charCreationInfo.bl_backgrounds",
	        "charCreationInfo.bl_accessories",
	        "charCreationInfo.bl_costumes",
	        "charCreationInfo.bl_lights",
	        "charCreationInfo.races",
	        "charCreationInfo.ancestries",
	        "charCreationInfo.schools",
	        "charCreationInfo.attributes",
	        "charCreationInfo.bl_beards",
	        "charCreationInfo.bl_skins",
	        "charCreationInfo.bl_lipsticks",
	        "charCreationInfo.bl_makeups"
        };

        public static int CreateCharacterCacheTableSize = CreateCharacterCacheTable.Length;

        public static string[] CharacterAppearanceCacheTable = new string[]
        {
           	"charCreationInfo.eyebrows",
	        "charCreationInfo.eyes",
	        "charCreationInfo.decos",
	        "charCreationInfo.hairs",
	        "charCreationInfo.backgrounds",
	        "charCreationInfo.accessories",
	        "charCreationInfo.costumes",
	        "charCreationInfo.lights",
	        "charCreationInfo.makeups",
	        "charCreationInfo.beards",
	        "charCreationInfo.skins",
	        "charCreationInfo.lipsticks"
        };

        public static int CharacterAppearanceCacheTableSize = CharacterAppearanceCacheTable.Length;
*/
        private Dictionary<string, PyObject> mCacheData = new Dictionary<string, PyObject>();
        private PyDict mCacheHints = new PyDict();

        public bool Exists(string name)
        {
	        return this.mCacheData.ContainsKey(name);
        }

        public PyObject Get(string name)
        {
            return this.mCacheData[name];
        }

        public void Store(string name, PyObject data, long timestamp)
        {
            Log.Info("Cache", $"Saving cache data for {name}");
            
            CacheInfo info = CacheInfo.FromPyObject(name, data, timestamp, Program.NodeID);
            
            // save cache hint
            this.mCacheHints.Set(name, info.Encode ());
            // save cache object
            this.mCacheData[name] = PyCachedObject.FromCacheInfo(info, data).Encode();
        }

        public PyDict GetHints(string[] list)
        {
	        PyDict hints = new PyDict();

	        foreach(string name in list)
				hints.Set(name, this.GetHint(name));
	        
	        return hints;
        }

        public PyObject GetHint(string name)
        {
	        return this.mCacheHints.Get(name);
        }

        private void Load(string name, string query, CacheObjectType type)
        {
	        Log.Debug("Cache", $"Loading cache data for {name} of type {type}");
	        
            MySqlDataReader reader = null;
            MySqlConnection connection = null;

            try
            {
	            Database.Query(ref reader, ref connection, query);
	            PyObject cacheObject = null;

	            switch (type)
	            {
		            case CacheObjectType.Rowset:
			            cacheObject = DBUtils.DBResultToRowset(ref reader);
			            break;
		            case CacheObjectType.CRowset:
			            cacheObject = DBUtils.DBResultToCRowset(ref reader);
			            break;
		            case CacheObjectType.TupleSet:
			            cacheObject = DBUtils.DBResultToTupleSet(ref reader);
			            break;
		            case CacheObjectType.PackedRowList:
			            cacheObject = DBUtils.DBResultToPackedRowList(ref reader);
			            break;
	            }
	            
	            Store(name, cacheObject, DateTime.Now.ToFileTimeUtc());
            }
            catch (Exception e)
            {
	            Log.Error("Cache", $"Cannot generate cache data for {name}");
	            throw;
            }
        }

        public void Load(string[] names, string[] queries, CacheObjectType[] types)
        {
            if (names.Length != queries.Length || names.Length != types.Length)
                throw new ArgumentOutOfRangeException("names", "names, queries and types do not match in size");

            for (int i = 0; i < names.Length; i++)
            {
                Load(names[i], queries[i], types[i]);
            }
        }

        public CacheStorage(DatabaseConnection db) : base(db)
        {
        }
    }
}
