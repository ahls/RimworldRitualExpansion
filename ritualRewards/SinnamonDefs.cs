using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Sinnamon_Ritual
{
	public class Sinnamon_RitualExtension : DefModExtension
	{
		public List<MemeDef> forbiddenMemeAny;
	}

	public class IncidentWorker_SmallAmbrosiaSprout : IncidentWorker
	{
		// Token: 0x060048F5 RID: 18677 RVA: 0x001823B0 File Offset: 0x001805B0
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}
			Map map = (Map)parms.target;
			IntVec3 intVec;
			return map.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNow && this.TryFindRootCell(map, out intVec);
		}

		// Token: 0x060048F6 RID: 18678 RVA: 0x001823F4 File Offset: 0x001805F4
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			IntVec3 intVec;
			if (!this.TryFindRootCell(map, out intVec))
			{
				return false;
			}
			Thing thing = null;
			int randomInRange = Rand.Range((int)parms.points%100, (int)parms.points/100);
			Predicate<IntVec3> intVecPred;
			for (int i = 0; i < randomInRange; i++)
			{
				IntVec3 root = intVec;
				Map map2 = map;
				int radius = 6;
				Predicate<IntVec3> extraValidator = (intVecPred = ((IntVec3 x) => this.CanSpawnAt(x, map)));
				
				IntVec3 intVec2;
				if (!CellFinder.TryRandomClosewalkCellNear(root, map2, radius, out intVec2, extraValidator))
				{
					break;
				}
				Plant plant = intVec2.GetPlant(map);
				if (plant != null)
				{
					plant.Destroy(DestroyMode.Vanish);
				}
				Thing thing2 = GenSpawn.Spawn(ThingDefOf.Plant_Ambrosia, intVec2, map, WipeMode.Vanish);
				if (thing == null)
				{
					thing = thing2;
				}
			}
			if (thing == null)
			{
				return false;
			}
			base.SendStandardLetter(parms, thing, Array.Empty<NamedArgument>());
			return true;
		}

		// Token: 0x060048F7 RID: 18679 RVA: 0x001824D4 File Offset: 0x001806D4
		private bool TryFindRootCell(Map map, out IntVec3 cell)
		{
			return CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (IntVec3 x) => this.CanSpawnAt(x, map) && x.GetRoom(map).CellCount >= 64, map, out cell);
		}

		// Token: 0x060048F8 RID: 18680 RVA: 0x00182510 File Offset: 0x00180710
		private bool CanSpawnAt(IntVec3 c, Map map)
		{
			if (!c.Standable(map) || c.Fogged(map) || map.fertilityGrid.FertilityAt(c) < ThingDefOf.Plant_Ambrosia.plant.fertilityMin || !c.GetRoom(map).PsychologicallyOutdoors || c.GetEdifice(map) != null || !PlantUtility.GrowthSeasonNow(c, map, false))
			{
				return false;
			}
			Plant plant = c.GetPlant(map);
			if (plant != null && plant.def.plant.growDays > 10f)
			{
				return false;
			}
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (thingList[i].def == ThingDefOf.Plant_Ambrosia)
				{
					return false;
				}
			}
			return true;
		}


		// Token: 0x04002CC6 RID: 11462
		private const int MinRoomCells = 32;

		// Token: 0x04002CC7 RID: 11463
		private const int SpawnRadius = 6;
	}
}
