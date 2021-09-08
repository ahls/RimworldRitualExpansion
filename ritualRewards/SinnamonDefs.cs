using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;
namespace Sinnamon_Ritual
{
	public static class Utilities
	{
		public static string reduceGameCondition(List<GameCondition> currentConditions,HashSet<string> reducingGC, HashSet<string> endingGC , int divider, string reducingMessage,string endingMessage)
		{
			string affectedEvents = "";
			foreach (var condition in currentConditions)
			{
				//heat waㅍㄷ 나 toxic fallout 지속시간 반감
				if (reducingGC != null && reducingGC.Contains(condition.def.defName))
				{
					affectedEvents += reducingMessage.Translate(condition.def.label);
					condition.Duration /= divider;
				}
				if (endingGC != null &&endingGC.Contains(condition.def.defName))
				{
					affectedEvents += endingMessage.Translate(condition.def.label);
					condition.End();
				}
			}
			return affectedEvents;
		}

	}

	public class Sinnamon_GameCondition_Aurora : GameCondition
	{
		private int curColorIndex = -1;
		private int prevColorIndex = -1;
		private float curColorTransition;
		private static readonly Color[] Colors = new Color[]
		{
			new Color(0f, 0.5f, 0f),
			new Color(0.1f, 0.5f, 0f),
			new Color(0f, 0.5f, 0.2f),
			new Color(0.3f, 0.5f, 0.3f),
			new Color(0f, 0.2f, 0.5f),
			new Color(0f, 0f, 0.5f),
			new Color(0.5f, 0f, 0f),
			new Color(0.3f, 0f, 0.5f)
		};
		public override void Init()
		{
			base.Init();
			this.curColorIndex = Rand.Range(0, Sinnamon_GameCondition_Aurora.Colors.Length);
			this.prevColorIndex = this.curColorIndex;
			this.curColorTransition = 1f;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.curColorIndex, "curColorIndex", 0, false);
			Scribe_Values.Look<int>(ref this.prevColorIndex, "prevColorIndex", 0, false);
			Scribe_Values.Look<float>(ref this.curColorTransition, "curColorTransition", 0f, false);
		}
		public Color CurrentColor
		{
			get
			{
				return Color.Lerp(Sinnamon_GameCondition_Aurora.Colors[this.prevColorIndex], Sinnamon_GameCondition_Aurora.Colors[this.curColorIndex], this.curColorTransition);
			}
		}
		private int TransitionDurationTicks
		{
			get
			{
				if (!base.Permanent)
				{
					return 280;
				}
				return 3750;
			}
		}
		public override int TransitionTicks
		{
			get
			{
				return 200;
			}
		}
		private int GetNewColorIndex()
		{
			return (from x in Enumerable.Range(0, Sinnamon_GameCondition_Aurora.Colors.Length)
					where x != this.curColorIndex
					select x).RandomElement<int>();
		}
		public override void GameConditionTick()
		{
			this.curColorTransition += 1f / (float)this.TransitionDurationTicks;
			if (this.curColorTransition >= 1f)
			{
				this.prevColorIndex = this.curColorIndex;
				this.curColorIndex = this.GetNewColorIndex();
				this.curColorTransition = 0f;
			}
		}
		public override SkyTarget? SkyTarget(Map map)
		{
			Color currentColor = this.CurrentColor;
			SkyColorSet colorSet = new SkyColorSet(Color.Lerp(Color.white, currentColor, 0.075f) * this.Brightness(map), new Color(0.92f, 0.92f, 0.92f), Color.Lerp(Color.white, currentColor, 0.025f) * this.Brightness(map), 1f);
			return new SkyTarget?(new SkyTarget(Mathf.Max(GenCelestial.CurCelestialSunGlow(map), 0.25f), colorSet, 1f, 1f));
		}
		private float Brightness(Map map)
		{
			return Mathf.Max(0.73f, GenCelestial.CurCelestialSunGlow(map));
		}
		// Token: 0x0600483D RID: 18493 RVA: 0x000D4E09 File Offset: 0x000D3009
		public override float SkyGazeChanceFactor(Map map)
		{
			return 8f;
		}

		// Token: 0x0600483E RID: 18494 RVA: 0x0017EF07 File Offset: 0x0017D107
		public override float SkyGazeJoyGainFactor(Map map)
		{
			return 5f;
		}

		// Token: 0x0600483F RID: 18495 RVA: 0x0017EF0E File Offset: 0x0017D10E
		public override float SkyTargetLerpFactor(Map map)
		{
			return GameConditionUtility.LerpInOutValue(this, (float)this.TransitionTicks, 1f);
		}
	}
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
