﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UI.CharacterCreator;
using Random = UnityEngine.Random;

namespace Systems.Character
{
/// <summary>
/// Class containing all character preferences for a player
/// Includes appearance, job preferences etc...
/// </summary>
public class CharacterSheet
{
	// TODO: all of the in-game appearance variables should probably be refactored into a separate class which can
	// then be used in PlayerScript since job preferences are only needed at round start in ConnectedPlayer

	// IMPORTANT: these fields use primitive types (int, string... etc) so they can be sent  over the network with
	// RPCs and Commands without needing to serialise them to JSON!
	public const int MAX_NAME_LENGTH = 26; // Arbitrary limit, but 26 is the max the current UI can fit

	public string Name = "Cuban Pete";
	public string AiName = "R.O.B.O.T.";
	public BodyType BodyType = BodyType.Male;
	public ClothingStyle ClothingStyle = ClothingStyle.JumpSuit;
	public BagStyle BagStyle = BagStyle.Backpack;
	public PlayerPronoun PlayerPronoun = PlayerPronoun.He_him;
	public int Age = 22;
	public Speech Speech = Speech.None;
	public string SkinTone = "#ffe0d1";
	public List<CustomisationStorage> SerialisedBodyPartCustom = new List<CustomisationStorage>();
	public List<ExternalCustomisation> SerialisedExternalCustom = new List<ExternalCustomisation>();

	public string Species = "Human";
	public JobPrefsDict JobPreferences = new JobPrefsDict();
	public AntagPrefsDict AntagPreferences = new AntagPrefsDict();

	[Serializable]
	public class CustomisationClass
	{
		public string SelectedName = "None";
		public string Colour = "#ffffff";
	}

	public override string ToString()
	{
		var sb = new StringBuilder($"{Name}'s character sheet:\n", 300);
		sb.AppendLine($"Name: {Name}");
		sb.AppendLine($"AiName: {AiName}");
		sb.AppendLine($"ClothingStyle: {ClothingStyle}");
		sb.AppendLine($"BagStyle: {BagStyle}");
		sb.AppendLine($"Pronouns: {PlayerPronoun}");
		sb.AppendLine($"Age: {Age}");
		sb.AppendLine($"Speech: {Speech}");
		sb.AppendLine($"SkinTone: {SkinTone}");
		sb.AppendLine($"JobPreferences: \n\t{string.Join("\n\t", JobPreferences)}");
		sb.AppendLine($"AntagPreferences: \n\t{string.Join("\n\t", AntagPreferences)}");
		return sb.ToString();
	}

	/// <summary>
	/// Does nothing if all the character's properties are valid
	/// <exception cref="InvalidOperationException">If the character settings are not valid</exception>
	/// </summary>
	public void ValidateSettings()
	{
		ValidateName();
		ValidateAiName();
		ValidateJobPreferences();
	}

	/// <summary>
	/// Checks if the character name follows all rules
	/// </summary>
	/// <exception cref="InvalidOperationException">If the name not valid</exception>
	private void ValidateName()
	{
		if (String.IsNullOrWhiteSpace(Name))
		{
			throw new InvalidOperationException("Name cannot be blank");
		}

		if (Name.Length > MAX_NAME_LENGTH)
		{
			throw new InvalidOperationException("Name cannot exceed " + MAX_NAME_LENGTH + " characters");
		}
	}

	/// <summary>
	/// Checks if the character Ai name follows all rules
	/// </summary>
	/// <exception cref="InvalidOperationException">If the name not valid</exception>
	private void ValidateAiName()
	{
		if (String.IsNullOrWhiteSpace(AiName))
		{
			AiName = "R.O.B.O.T.";
		}

		if (AiName.Length > MAX_NAME_LENGTH)
		{
			throw new InvalidOperationException("Name cannot exceed " + MAX_NAME_LENGTH + " characters");
		}
	}

	/// <summary>
	/// Checks if the job preferences have more than one high priority set
	/// </summary>
	/// <exception cref="InvalidOperationException">If the job preferences are not valid</exception>
	private void ValidateJobPreferences()
	{
		if (JobPreferences.Count(jobPref => jobPref.Value == Priority.High) > 1)
		{
			throw new InvalidOperationException("Cannot have more than one job set to high priority");
		}
	}

	/// <summary>
	/// Returns a possessive string (i.e. "their", "his", "her") for the provided gender enum.
	/// </summary>
	public string TheirPronoun(PlayerScript script)
	{
		if (script.Equipment != null &&
		     script.Equipment.GetPlayerNameByEquipment() == "Unknown" && script.Equipment.IsIdentityObscured())
		{
			return "their";
		}

		switch (PlayerPronoun)
		{
			case PlayerPronoun.He_him:
				return "his";
			case PlayerPronoun.She_her:
				return "her";
			default:
				return "their";
		}
	}

	/// <summary>
	/// Returns a personal pronoun string (i.e. "he", "she", "they") for the provided gender enum.
	/// </summary>
	public string TheyPronoun(PlayerScript script)
	{
		if (script.Equipment.GetPlayerNameByEquipment() == "Unknown" && script.Equipment.IsIdentityObscured())
		{
			return "they";
		}
		switch (PlayerPronoun)
		{
			case PlayerPronoun.He_him:
				return "he";
			case PlayerPronoun.She_her:
				return "she";
			default:
				return "they";
		}
	}

	/// <summary>
	/// Returns an object pronoun string (i.e. "him", "her", "them") for the provided gender enum.
	/// </summary>
	public string ThemPronoun(PlayerScript script)
	{
		if (script.Equipment.GetPlayerNameByEquipment() == "Unknown" && script.Equipment.IsIdentityObscured())
		{
			return "them";
		}
		switch (PlayerPronoun)
		{
			case PlayerPronoun.He_him:
				return "him";
			case PlayerPronoun.She_her:
				return "her";
			default:
				return "them";
		}
	}

	/// <summary>
	/// Returns an object pronoun string (i.e. "he's", "she's", "they're") for the provided gender enum.
	/// </summary>
	public string TheyrePronoun(PlayerScript script)
	{
		if (script.Equipment.GetPlayerNameByEquipment() == "Unknown" && script.Equipment.IsIdentityObscured())
		{
			return "they're";
		}
		switch (PlayerPronoun)
		{
			case PlayerPronoun.He_him:
				return "he's";
			case PlayerPronoun.She_her:
				return "she's";
			default:
				return "they're";
		}
	}

	/// <summary>
	/// Returns an object pronoun string (i.e. "himself", "herself", "themself") for the provided gender enum.
	/// </summary>
	public string ThemselfPronoun(PlayerScript script)
	{
		if (script.Equipment.GetPlayerNameByEquipment() == "Unknown" && script.Equipment.IsIdentityObscured())
		{
			return "themself";
		}
		switch (PlayerPronoun)
		{
			case PlayerPronoun.He_him:
				return "himself";
			case PlayerPronoun.She_her:
				return "herself";
			default:
				return "themself";
		}
	}

	public string IsPronoun(PlayerScript script)
	{
		if (script.Equipment.GetPlayerNameByEquipment() == "Unknown" && script.Equipment.IsIdentityObscured())
		{
			return "are";
		}
		switch (PlayerPronoun)
		{
			case PlayerPronoun.He_him:
			case PlayerPronoun.She_her:
				return "is";
			case PlayerPronoun.They_them:
			default:
				return "are";
		}
	}

	public string HasPronoun(PlayerScript script)
	{
		if (script.Equipment.GetPlayerNameByEquipment() == "Unknown" && script.Equipment.IsIdentityObscured())
		{
			return "have";
		}
		switch (PlayerPronoun)
		{
			case PlayerPronoun.He_him:
			case PlayerPronoun.She_her:
				return "has";
			case PlayerPronoun.They_them:
			default:
				return "have";
		}
	}

	public Gender GetGender()
	{
		switch (BodyType)
		{
			case BodyType.Male:
				return Gender.Male;
			case BodyType.Female:
				return Gender.Female;
			default:
				return Gender.NonBinary;
		}
	}

	public PlayerHealthData GetRaceSo()
	{
		return RaceSOSingleton.Instance.Races.FirstOrDefault(x => x.name == Species);
	}

	#region StaticCustomizationFunctions

	public static CharacterSheet GenerateRandomCharacter( )
	{
		CharacterSheet character = new CharacterSheet();

		character.Species = RaceSOSingleton.Instance.Races.PickRandom().name;
		character.BodyType = character.BodyType.PickRandom();
		character.Age = Random.Range(19, 84);
		character.SkinTone = GetRandomSkinTone(character.Species);
		character.Name = character.Species == "Lizard" ? StringManager.GetRandomLizardName() : StringManager.GetRandomName();
		character.Speech = DMMath.Prob(35) ? character.Speech.PickRandom() : Speech.None;
		character.PlayerPronoun = character.PlayerPronoun.PickRandom();
		character.ClothingStyle = character.ClothingStyle.PickRandom();
		character.BagStyle = character.BagStyle.PickRandom();

		character.AiName = "R.O.B.O.T."; // TODO: random names.

		character.SerialisedBodyPartCustom = new List<CustomisationStorage>(); // TODO ask bod
		character.SerialisedExternalCustom = GetRandomUnderwear(character.Species);

		return character;
	}

	private static Color GetRandomColor()
	{
		return new Color(Random.Range(0.1f, 1f), Random.Range(0.1f, 1f), Random.Range(0.1f, 1f));
	}

	public static string GetRandomSkinTone(string speciesName = "Human")
	{
		if (RaceSOSingleton.TryGetRaceByName(speciesName, out PlayerHealthData race))
		{
			List<Color> raceSkinColors = race.Base.SkinColours;
			if (raceSkinColors.Count > 0)
			{
				return $"#{ColorUtility.ToHtmlStringRGB(raceSkinColors.PickRandom())}";
			}
		}

		return $"#{ColorUtility.ToHtmlStringRGB(GetRandomColor())}";
	}

	private static List<ExternalCustomisation> GetRandomUnderwear(string speciesName = "Human")
	{
		var externalCustomisations = new List<ExternalCustomisation>();

		if (RaceSOSingleton.TryGetRaceByName(speciesName, out PlayerHealthData race) == false)
		{
			return externalCustomisations;
		}

		void Logic(CustomisationAllowedSetting setting)
		{
			PlayerCustomisationData customizationToAdd = setting.CustomisationGroup.PlayerCustomisations.PickRandom();
			ExternalCustomisation newExternalCustomisation = new ExternalCustomisation();
			newExternalCustomisation.Key = customizationToAdd.name;
			newExternalCustomisation.SerialisedValue = SerialiseCustomizationData(customizationToAdd);
			externalCustomisations.Add(newExternalCustomisation);
		}

		foreach (CustomisationAllowedSetting customisation in race.Base.CustomisationSettings)
		{
			if (customisation.CustomisationGroup.name == "PlayerUnderShirt") Logic(customisation);
			if (customisation.CustomisationGroup.name == "PlayerUnderWear") Logic(customisation);
			if (customisation.CustomisationGroup.name == "PlayerSocks") Logic(customisation);
		}

		return externalCustomisations;
	}

	private static CustomisationClass SerialiseCustomizationData(PlayerCustomisationData data)
	{
		var newcurrentSetting = new CustomisationClass();
		newcurrentSetting.Colour = $"#{ColorUtility.ToHtmlStringRGB(GetRandomColor())}";
		newcurrentSetting.SelectedName = data.Name;
		return newcurrentSetting;
	}

	#endregion
}
}
