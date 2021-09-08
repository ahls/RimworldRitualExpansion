using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using Verse;
using RimWorld;

namespace Sinnamon_Ritual
{
	//돌개바람
	public class RitualAttachableOutcomeEffectWorker_WindBless : RitualAttachableOutcomeEffectWorker
	{
		//바람출력을 높여주고, heatwave랑 toxic fallout 지속시간 get 하고 낮춤.
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			HashSet<string> EventsToReduceDuration = new HashSet<string> { "HeatWave", "GlobalWarming" };
			List<GameCondition> currentConditions = new List<GameCondition>();
			jobRitual.Map.GameConditionManager.GetAllGameConditionsAffectingMap(jobRitual.Map, currentConditions);
			int divider = outcome.BestPositiveOutcome(jobRitual) ? 4 : 2;
			string affectedEvents = Utilities.reduceGameCondition(currentConditions, EventsToReduceDuration, null, divider, "WindBlessAffectedEvents", string.Empty);

			//바람 출력 높여줌

			GameCondition_ForceWeather gc = (GameCondition_ForceWeather)Activator.CreateInstance(typeof(GameCondition_ForceWeather));
			gc.startTick = Find.TickManager.TicksGame;
			gc.def = GameConditionDef.Named("Sinnamon_WindBlessing");
			gc.Duration = outcome.BestPositiveOutcome(jobRitual) ? 120000 : 60000;
			gc.uniqueID = Find.UniqueIDsManager.GetNextGameConditionID();
			gc.weather = WeatherDef.Named("Sinnamon_Windy");

			jobRitual.Map.GameConditionManager.RegisterCondition(gc);


			extraOutcomeDesc = this.def.letterInfoText + affectedEvents;
		}
	}

	//비바라기
	public class RitualAttachableOutcomeEffectWorker_RainBless : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			HashSet<string> EventsToReduceDuration = new HashSet<string> { "ToxicFallout", "VolcanicWinter" };
			HashSet<string> EventsToEnd = new HashSet<string> { "Drought" };
			List<GameCondition> currentConditions = new List<GameCondition>();
			jobRitual.Map.GameConditionManager.GetAllGameConditionsAffectingMap(jobRitual.Map, currentConditions);
			int divider = outcome.BestPositiveOutcome(jobRitual) ? 4 : 2;
			string affectedEvents = Utilities.reduceGameCondition(currentConditions, EventsToReduceDuration, EventsToEnd, divider, "RainBlessAffectedEvents", "RainBlessEndedEvents");

			//비 내리게 함.
			GameCondition_ForceWeather gc = (GameCondition_ForceWeather)Activator.CreateInstance(typeof(GameCondition_ForceWeather));
			gc.startTick = Find.TickManager.TicksGame;
			gc.def = GameConditionDef.Named("Sinnamon_RainBlessing");
			gc.Duration = outcome.BestPositiveOutcome(jobRitual) ? 120000 : 60000;
			gc.uniqueID = Find.UniqueIDsManager.GetNextGameConditionID();
			gc.weather = WeatherDef.Named("Rain");

			jobRitual.Map.GameConditionManager.RegisterCondition(gc);


			extraOutcomeDesc = this.def.letterInfoText + affectedEvents;
		}
	}

	//태양 만세
	public class RitualAttachableOutcomeEffectWorker_SunBless : RitualAttachableOutcomeEffectWorker
	{

		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			HashSet<string> EventsToReduceDuration = new HashSet<string> { "ColdSnap", "IceAge", "LongNight" };
			HashSet<string> EventsToEnd = new HashSet<string> { "Eclipse" };
			List<GameCondition> currentConditions = new List<GameCondition>();
			jobRitual.Map.GameConditionManager.GetAllGameConditionsAffectingMap(jobRitual.Map, currentConditions);
			int divider = outcome.BestPositiveOutcome(jobRitual) ? 4 : 2;
			string affectedEvents = Utilities.reduceGameCondition(currentConditions, EventsToReduceDuration, EventsToEnd, divider, "SunBlessAffectedEvents", "SunBlessEndedEvents");

			/*
			GameCondition onGoingEclipse = jobRitual.Map.gameConditionManager.GetActiveCondition<GameCondition_NoSunlight>();
			if (onGoingEclipse != null)
			{
			}*/

			GameCondition_ForceWeather gc = (GameCondition_ForceWeather)Activator.CreateInstance(typeof(GameCondition_ForceWeather));
			gc.startTick = Find.TickManager.TicksGame;
			gc.def = GameConditionDef.Named("Sinnamon_SunBlessing");
			gc.Duration = outcome.BestPositiveOutcome(jobRitual) ? 120000 : 60000;
			gc.uniqueID = Find.UniqueIDsManager.GetNextGameConditionID();
			gc.weather = WeatherDefOf.Clear;

			jobRitual.Map.GameConditionManager.RegisterCondition(gc);

			extraOutcomeDesc = this.def.letterInfoText + affectedEvents;
		}
	}

	// 일식
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

			if (outcome.BestPositiveOutcome(jobRitual))
			{
				GameCondition_Aurora aurora = (GameCondition_Aurora)GameConditionMaker.MakeCondition(GameConditionDefOf.Aurora, 30000);
				jobRitual.Map.GameConditionManager.RegisterCondition(aurora);
				extraOutcomeDesc = "Sinnamon_EclipseWithAurora".Translate();
			}
			else
			{
				extraOutcomeDesc = this.def.letterInfoText;
			}
		}
	}

	//오로라
	public class RitualAttachableOutcomeEffectWorker_Aurora : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			int duration = outcome.BestPositiveOutcome(jobRitual) ? 4 : 2;
			Sinnamon_GameCondition_Aurora aurora = (Sinnamon_GameCondition_Aurora)GameConditionMaker.MakeCondition(GameConditionDefOf.Aurora, duration * 60000);
			jobRitual.Map.GameConditionManager.RegisterCondition(aurora);

			extraOutcomeDesc = this.def.letterInfoText + $" {duration} days.";
		}
	}
	public class RitualAttachableOutcomeEffectWorker_Ambrosia : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			IncidentParms parms = new IncidentParms { target = jobRitual.Map, points = outcome.BestPositiveOutcome(jobRitual) ? 1510 : 1005 };
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
			IncidentParms parms = new IncidentParms { target = jobRitual.Map, points = (jobRitual.Map.PlayerWealthForStoryteller / 500) };
			IncidentDefOf.RaidEnemy.Worker.TryExecute(parms);
			extraOutcomeDesc = this.def.letterInfoText;
		}
	}
	public class RitualAttachableOutcomeEffectWorker_Strip : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			extraOutcomeDesc = "";
			bool strippedAny = false;
			float chance = outcome.BestPositiveOutcome(jobRitual) ? 0.33f : 0.75f;
			IEnumerable<Pawn> enemies = from x in jobRitual.Map.mapPawns.AllPawnsSpawned
										where x.Faction.HostileTo(Faction.OfPlayer)
										select x;
			if (enemies.Count() == 0)
			{
				return;
			}
			foreach (var x in enemies)
			{
				if (Rand.Chance(chance))
				{
					x.apparel.DropAll(x.PositionHeld);
					strippedAny = true;
				}
			}
			if (strippedAny)
			{
				extraOutcomeDesc = this.def.letterInfoText + (outcome.BestPositiveOutcome(jobRitual) ? "Sinnamon_stripGood".Translate() : "Sinnamon_stripGreat".Translate());
			}
		}
	}
	public class RitualAttachableOutcomeEffectWorker_CallVenerated : RitualAttachableOutcomeEffectWorker
	{
		private Pawn spawnAnimal(PawnKindDef kind, Gender? gender, IntVec3 loc, Map map)
		{
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, null, PawnGenerationContext.NonPlayer, -1, false, false, false, false, false, false, 1f, false, false, false, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, gender, null, null, null, null, null, false, false, false));
			GenSpawn.Spawn(pawn, loc, map, Rot4.Random, WipeMode.Vanish, false);
			return pawn;
		}
		private float selectionChance(PawnKindDef k)
		{
			return 1 / k.race.BaseMarketValue;
		}
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{

			extraOutcomeDesc = "";

			bool isBestOutcome = outcome.BestPositiveOutcome(jobRitual);
			if (!isBestOutcome && Rand.Chance(0.7f))
				return;
			Map map = jobRitual.Map;
			IntVec3 loc;
			if (RCellFinder.TryFindRandomPawnEntryCell(out loc, map, CellFinder.EdgeRoadChance_Animal, false, null))
			{
				if (jobRitual.Ritual.ideo.VeneratedAnimals.Count == 0)
				{
					return;
				}
				IEnumerable<PawnKindDef> venAnimals = (from x in DefDatabase<PawnKindDef>.AllDefs
													   where jobRitual.Ritual.ideo.VeneratedAnimals.Contains(x.race)
													   select x);
				PawnKindDef venAnimal;
				if (!isBestOutcome)
				{
					venAnimal = (from x in venAnimals
								 where map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(x.race)
								 select x).RandomElementByWeight((PawnKindDef k) => selectionChance(k));
					if (venAnimal == null)
					{
						extraOutcomeDesc = this.def.letterInfoText + "Sinnamon_VeneratedAnimalFailTemperature".Translate(outcome.label);
						return;
					}
				}
				venAnimal = (from x in DefDatabase<PawnKindDef>.AllDefs
							 where jobRitual.Ritual.ideo.VeneratedAnimals.Contains(x.race)
							 select x).RandomElementByWeight((PawnKindDef k) => selectionChance(k));


				if (isBestOutcome)
				{
					//if the weather is nice, spawn a pair.
					if (map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(venAnimal.race))
					{
						extraOutcomeDesc = "Sinnamon_VeneratedAnimalCalledPair".Translate(venAnimal.label);

						letterLookTargets.targets.Add(spawnAnimal(venAnimal, Gender.Male, loc, map));
						letterLookTargets.targets.Add(spawnAnimal(venAnimal, Gender.Female, loc, map));
					}
					//if the weather is not acceptable, only spawn one.
					else
					{
						extraOutcomeDesc = "Sinnamon_VeneratedAnimalCalledBadWeather".Translate(venAnimal.label);
						letterLookTargets.targets.Add(spawnAnimal(venAnimal, null, loc, map));
					}
				}
				else
				{
					letterLookTargets.targets.Add(spawnAnimal(venAnimal, null, loc, map));

					extraOutcomeDesc = "Sinnamon_VeneratedAnimalCalled".Translate(venAnimal.label);
				}
			}
			else
			{
				extraOutcomeDesc = this.def.letterInfoText + "Sinnamon_VeneratedAnimalFailNoEntry".Translate();
			}
		}
	}
	public class RitualAttachableOutcomeEffectWorker_Meteor : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{

			IncidentParms parms = new IncidentParms { target = jobRitual.Map };
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
		const float bestOutcome = 10;
		const float regularOutcome = 5;
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			extraOutcomeDesc = null;
			Map map = jobRitual.Map;
			IntVec3 intVec = FindRootTunnelLoc(map);
			if (intVec == IntVec3.Invalid)
			{
				extraOutcomeDesc = "Sinnamon_InsectNoExit".Translate();
				return;
			}

			//크게 성공했으면 두배로 많은 양 소환
			bool isBestOutcome = outcome.BestPositiveOutcome(jobRitual);
			int numLimit = isBestOutcome ? (int)bestOutcome : (int)regularOutcome;
			float sizeLimit = isBestOutcome ? bestOutcome : regularOutcome;

			List<PawnKindDef> possibleList = GetPossibleInsectList(map);
			while (numLimit > 0 && sizeLimit > 0)
			{
				numLimit--;
				sizeLimit -= SpawnInsects(intVec, map, possibleList);
			}
			extraOutcomeDesc = this.def.letterInfoText;
			letterLookTargets = new LookTargets(intVec, map);
		}
		private float SpawnInsects(IntVec3 intVec, Map map, List<PawnKindDef> insectList)
		{
			IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 3, null);
			PawnKindDef bugPawn = insectList.RandomElement();
			Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(bugPawn, null, PawnGenerationContext.NonPlayer, -1, false, false, false, false, false, false, 1f, false, false, false, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null, null, false, false, false));
			GenSpawn.Spawn(pawn, loc, map, Rot4.Random, WipeMode.Vanish, false);
			return bugPawn.RaceProps.baseBodySize;
		}
		private List<PawnKindDef> GetPossibleInsectList(Map map)
		{
			Func<List<CompProperties>, bool> validator = delegate (List<CompProperties> compList)
			{
				foreach (var cp in compList)
				{
					Type currentComp = cp.compClass;
					if (currentComp.Name == "CompUntameable" || currentComp.Name == "CompFloating")
					{
						return false;
					}
				}
				return true;
			};
			return (from x in DefDatabase<PawnKindDef>.AllDefs
					where (x.RaceProps.Insect && !x.defName.StartsWith("VFEI_VatGrown") && x.RaceProps.wildness <= 0.8 &&
					map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(x.race) && validator(x.race.comps))
					select x).ToList();

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
			for (int i = list.Count - 1; i >= 0; i--)
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




	public class RitualAttachableOutcomeEffectWorker_Harvest : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			float growingAmount = outcome.BestPositiveOutcome(jobRitual) ? 20 : 10;
			HashSet<string> plantKinds = new HashSet<string>();
			int affectedPlants = 0;
			List<Zone> list = jobRitual.Map.zoneManager.AllZones;
			List<Zone> farmAreas = (from x in list
									where x.GetType() == typeof(Zone_Growing)
									select x).InRandomOrder().ToList();


			foreach (var farmArea in farmAreas)
			{
				string designatedCrop = ((Zone_Growing)farmArea).GetPlantDefToGrow().label;
				foreach (var cell in farmArea.cells)
				{
					Plant plant = cell.GetPlant(jobRitual.Map);
					if (plant != null)
					{
						if (plant.def.label == designatedCrop && plant.Growth < 1)
						{

							plantKinds.Add(plant.Label);
							affectedPlants++;
							if (plant.Growth > 0.5f)
							{
								growingAmount -= 1 - plant.Growth;
								plant.Growth = 1;

							}
							else
							{
								growingAmount -= 0.5f;
								plant.Growth += 0.5f;
							}
						}
						if (growingAmount < 0)
						{
							break;
						}
					}
				}
				if (growingAmount < 0)
				{
					break;
				}
			}
			

			extraOutcomeDesc = "Sinnamon_HarvestResult".Translate(affectedPlants, string.Join(", ", plantKinds));
		}
	}

	public class RitualAttachableOutcomeEffectWorker_TreeConnection : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{

			foreach (var colonist in totalPresence.Keys)
			{
				foreach (var connected in colonist.connections.ConnectedThings)
				{
					if(connected.def.defName == "Plant_TreeGauranlen")
                    {
						connected.TryGetComp<CompTreeConnection>().ConnectionStrength = 1;
                    }

				}
			}
			extraOutcomeDesc = this.def.letterInfoText;
		}
	}
	public class RitualAttachableOutcomeEffectWorker_Random : RitualAttachableOutcomeEffectWorker
	{
		public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
		{
			string outcomeDesc;
			int choices = Rand.Range(0, 10);
			RitualAttachableOutcomeEffectWorker worker;
			List<RitualAttachableOutcomeEffectWorker> possibleRewards = new List<RitualAttachableOutcomeEffectWorker>(); 
			foreach(var x in DefDatabase<RitualAttachableOutcomeEffectDef>.AllDefs)
            {
				if( x.defName == "Sinnamon_Random" ||
					x.defName == "Sinnamon_Aurora")
                {
					continue;
                }
				possibleRewards.Add(x.Worker);
            }
			worker = possibleRewards.RandomElement();
			worker.Apply(totalPresence, jobRitual, outcome, out outcomeDesc, ref letterLookTargets);

			extraOutcomeDesc = "Sinnamon_Random".Translate(worker.def.label) + outcomeDesc;
		}
	}
}
