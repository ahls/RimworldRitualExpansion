using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;
using RimWorld;

namespace Sinnamon_Ritual
{
	public class RitualAttachableOutcomeEffectWorker_WindBless : RitualAttachableOutcomeEffectWorker
	{

		//바람출력을 높여주고, heatwave랑 toxic fallout 지속시간 get 하고 낮춤.
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			List<GameCondition> currentConditions = new List<GameCondition>();
			jobRitual.Map.GameConditionManager.GetAllGameConditionsAffectingMap(jobRitual.Map, currentConditions);
			int divider = outcome.BestPositiveOutcome(jobRitual) ? 2 : 4;
			foreach (var condition in currentConditions)
			{
				//heat waㅍㄷ 나 toxic fallout 지속시간 반감
				if (condition.def.defName == "HeatWave" || condition.def.defName == "ToxicFallout")
				{
					condition.Duration /= divider;
				}
			}
			//바람 출력 높여줌

			GameCondition_ForceWeather gc = (GameCondition_ForceWeather)Activator.CreateInstance(typeof(GameCondition_ForceWeather));
			gc.startTick = Find.TickManager.TicksGame;
			gc.def = GameConditionDef.Named("Sinnamon_WindBlessing");
			gc.Duration = 60000;
			gc.uniqueID = Find.UniqueIDsManager.GetNextGameConditionID();
			gc.weather = WeatherDef.Named("Sinnamon_Windy");

			jobRitual.Map.GameConditionManager.RegisterCondition(gc);

			
			extraOutcomeDesc = this.def.letterInfoText;
		}
	}
	public class RitualAttachableOutcomeEffectWorker_SunBless : RitualAttachableOutcomeEffectWorker
	{

		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			List<GameCondition> currentConditions = new List<GameCondition>();
			jobRitual.Map.GameConditionManager.GetAllGameConditionsAffectingMap(jobRitual.Map, currentConditions);
			int divider = outcome.BestPositiveOutcome(jobRitual)?2:4;
			foreach (var condition in currentConditions)
			{
				if (condition.def.defName == "ColdSnap" || condition.def.defName == "VolcanicWinter")
				{
					condition.Duration /= divider;
				}
			}
			GameCondition onGoingEclipse = jobRitual.Map.gameConditionManager.GetActiveCondition<GameCondition_NoSunlight>();
			if (onGoingEclipse != null)
			{
				onGoingEclipse.End();
			}


			GameCondition_ForceWeather gc = (GameCondition_ForceWeather)Activator.CreateInstance(typeof(GameCondition_ForceWeather));
			gc.startTick = Find.TickManager.TicksGame;
			gc.def = GameConditionDef.Named("Sinnamon_SunBlessing");
			gc.Duration = 60000;
			gc.uniqueID = Find.UniqueIDsManager.GetNextGameConditionID();
			gc.weather = WeatherDefOf.Clear;

			jobRitual.Map.GameConditionManager.RegisterCondition(gc);

			extraOutcomeDesc = this.def.letterInfoText;
		}
	}
	public class RitualAttachableOutcomeEffectWorker_Eclipse : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			GameCondition onGoingFlare = jobRitual.Map.gameConditionManager.GetActiveCondition<GameCondition_DisableElectricity>();
			if (onGoingFlare != null)
			{
				onGoingFlare.End();
			}
			GameCondition_NoSunlight eclipse = (GameCondition_NoSunlight)GameConditionMaker.MakeCondition(GameConditionDefOf.Eclipse, 120000);

			jobRitual.Map.GameConditionManager.RegisterCondition(eclipse);
/*			
			IncidentParms parms = new IncidentParms { target = jobRitual.Map, sendLetter = false};
			IncidentDefOf.Eclipse.Worker.TryExecute(parms);*/
			extraOutcomeDesc = this.def.letterInfoText;

		}
	}
	public class RitualAttachableOutcomeEffectWorker_Ambrosia : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			IncidentParms parms = new IncidentParms { target = jobRitual.Map ,points = outcome.BestPositiveOutcome(jobRitual)?1510:1005};
			IncidentDef incidentDef = IncidentDef.Named("Sinnamon_SmallAmbrosiaSprout");
			if (!incidentDef.Worker.CanFireNow(parms))
            {
				extraOutcomeDesc = "Sinnamon_AmbrosiaFailed";
				return;
            }
			incidentDef.Worker.TryExecute(parms);
			extraOutcomeDesc = this.def.letterInfoText;
		}
	}
	public class RitualAttachableOutcomeEffectWorker_Taunt : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			IncidentParms parms = new IncidentParms { target = jobRitual.Map, points = (jobRitual.Map.PlayerWealthForStoryteller / 500)};
			IncidentDefOf.RaidEnemy.Worker.TryExecute(parms);
			Log.Warning("Raid has bee ncalled");
			extraOutcomeDesc = this.def.letterInfoText;
		}
	}
	public class RitualAttachableOutcomeEffectWorker_Meteor : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{

			IncidentParms parms = new IncidentParms { target = jobRitual.Map};
			IncidentDef.Named("MeteoriteImpact").Worker.TryExecute(parms);
			extraOutcomeDesc = this.def.letterInfoText;
		}
	}
	public class RitualAttachableOutcomeEffectWorker_SpaceshipChunk : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			IncidentParms parms = new IncidentParms { target = jobRitual.Map, sendLetter = false };
			IncidentDefOf.ShipChunkDrop.Worker.TryExecute(parms);
			extraOutcomeDesc = this.def.letterInfoText;
		}
	}
	public class RitualAttachableOutcomeEffectWorker_Trader : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			//상선 오게함
			IncidentParms parms = new IncidentParms { target = jobRitual.Map, sendLetter = false };
			IncidentDefOf.OrbitalTraderArrival.Worker.TryExecute(parms);
			extraOutcomeDesc = this.def.letterInfoText;
		}
	}
	public class RitualAttachableOutcomeEffectWorker_Insects : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			extraOutcomeDesc = null;
			Map map = jobRitual.Map;
			IntVec3 intVec = FindRootTunnelLoc(map);
			if(intVec == IntVec3.Invalid)
			{
				extraOutcomeDesc = "Sinnamon_InsectNoExit".Translate();
				return;
			}

			//크게 성공했으면 두배로 많은 양 소환
			bool isBestOutcome = outcome.BestPositiveOutcome(jobRitual);
			int numLimit = isBestOutcome?10:5;
			float sizeLimit = isBestOutcome?10:5f;


			while (numLimit > 0 && sizeLimit > 0)
			{
				numLimit--;
				sizeLimit -= SpawnInsects(intVec, map);
			}
			extraOutcomeDesc = this.def.letterInfoText;
			letterLookTargets = new LookTargets(intVec, map);
		}
		private float SpawnInsects(IntVec3 intVec,Map map)
		{
			IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 3, null);
			PawnKindDef bugPawn;
			TryFindRandomPawnKind(out bugPawn);
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(bugPawn, null, PawnGenerationContext.NonPlayer, -1, false, false, false, false, false, false, 1f, false, false, false, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false));
			GenSpawn.Spawn(pawn, loc, map, Rot4.Random, WipeMode.Vanish, false);
			return bugPawn.RaceProps.baseBodySize;
		}
		private bool TryFindRandomPawnKind(out PawnKindDef kind)
		{
			return (from x in DefDatabase<PawnKindDef>.AllDefs
					where x.RaceProps.Insect 
					select x).TryRandomElement(out kind);
		}
		private static IntVec3 FindRootTunnelLoc(Map map)
		{
			IntVec3 result;
			if (InfestationCellFinder.TryFindCell(out result, map))
			{
				return result;
			}
			Func<IntVec3, bool, bool> validator = delegate (IntVec3 x, bool canIgnoreRoof)
			{
				if (!x.Standable(map) || x.Fogged(map))
				{
					return false;
				}
				return true;
			};
			if (RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((IntVec3 x) => validator(x, false), map, out result))
			{
				return result;
			}
			return IntVec3.Invalid;
		}

	}
	public class RitualAttachableOutcomeEffectWorker_Dust : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			List<Thing> list = jobRitual.Map.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
			for (int i = list.Count-1; i >=0; i--)
			{
				Corpse corpse = (Corpse)list[i];
				if (corpse.GetRotStage() >= RotStage.Rotting)
				{
					corpse.DeSpawn(DestroyMode.Vanish);
					if (!corpse.Destroyed)
					{
						corpse.Destroy(DestroyMode.Vanish);
					}
					if (!corpse.Discarded)
					{
						corpse.Discard(false);
					}
				}
				
			}
			extraOutcomeDesc = this.def.letterInfoText;
		}
	}

}
