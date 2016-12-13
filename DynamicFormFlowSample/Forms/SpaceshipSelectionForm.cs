using DynamicFormFlowSample.Data;
using DynamicFormFlowSample.Models;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace DynamicFormFlowSample.Forms
{
    public class SpaceshipSelectionForm
    {
        public static IForm<Spaceship> BuildForm()
        {
            var builder = new FormBuilder<Spaceship>();

            return builder
                // The following is the simple way of creating a field - you can leave out the
                // validation, if it not needed. Hint: Use Prompt attribute in the property;
                // see the Size property in Spaceship class.
                .Field(nameof(Spaceship.Size),
                       validate: async (state, value) =>
                           await ValidateResponseAsync(value, state, nameof(Spaceship.Size)))

                // The following way of creating a field provides the best access to define its
                // behavior:
                //
                // - SetType: If you want to explicitly define the type of the property
                // - SetActive: Should we present this query?
                // - SetDefine: What values will be available?
                // - SetPrompt: How do we present the query?
                // - SetValidate: The actions taken after response
                //
                .Field(new FieldReflector<Spaceship>(nameof(Spaceship.Engines))
                    .SetType(typeof(Spaceship.EngineTypes))
                    .SetActive((state) => SetFieldActive(state, nameof(Spaceship.Engines)))
                    .SetDefine(async (state, field) => await SetOptionsForFieldsAsync(state, nameof(Spaceship.Engines), field))
                    .SetAllowsMultiple(true)
                    .SetPrompt(new PromptAttribute("What type of engines does the ship have? {||}"))
                    .SetValidate(async (state, value) => await ValidateResponseAsync(value, state, nameof(Spaceship.Engines))))

                // You could also replace the field above with the following, if you have no need
                // to define the values for the field or validate the response:
                //.Field("Engines")

                // Below we want only to allow one option. To achieve that we've defined a
                // different property where the value is stored in (Spaceship.Weapon instead
                // of Spaceship.Weapons).
                .Field(new FieldReflector<Spaceship>(nameof(Spaceship.Weapon))
                    .SetActive((state) => SetFieldActive(state, nameof(Spaceship.Weapons)))
                    .SetDefine(async (state, field) => await SetOptionsForFieldsAsync(state, nameof(Spaceship.Weapons), field))
                    .SetPrompt(new PromptAttribute("How about the weapons on the ship? {||}")
                    {
                        ChoiceStyle = ChoiceStyleOptions.Default
                    })
                    .SetValidate(async (state, value) => await ValidateResponseAsync(value, state, nameof(Spaceship.Weapons))))

                .Field(new FieldReflector<Spaceship>(nameof(Spaceship.Crew))
                    .SetActive((state) => SetFieldActive(state, nameof(Spaceship.Crew)))
                    .SetDefine(async (state, field) => await SetOptionsForFieldsAsync(state, nameof(Spaceship.Crew), field))
                    .SetPrompt(new PromptAttribute("What kind of crew typically runs the ship? {||}"))
                    .SetValidate(async (state, value) => await ValidateResponseAsync(value, state, nameof(Spaceship.Crew))))
                
                // Uncomment the following to present a confirmation prompt
                //.Confirm(generateMessage: (state) => GenerateConfirmMessageAsync(state))

                .Build();
        }

#pragma warning disable 1998
        /// <summary>
        /// Validates the response and does a new search with the updated parameters.
        /// </summary>
        /// <param name="response">The response from the user.</param>
        /// <param name="spaceshipState">The current state of the form (what details we have gathered to our spaceship).</param>
        /// <param name="propertyName">The name of the property queried.</param>
        /// <returns>The validation result.</returns>
        private static async Task<ValidateResult> ValidateResponseAsync(
            object response, Spaceship spaceshipState, string propertyName)
        {
            Spaceship spaceshipSearchFilter = new Spaceship(spaceshipState);
            object value = Spaceship.VerifyPropertyValue(response, propertyName);
            SpaceshipData spaceshipData = SpaceshipData.Instance;
            IList<Spaceship> matches = null;

            if (spaceshipSearchFilter.SetPropertyValue(value, propertyName))
            {
                matches = spaceshipData.SearchForPartialMatches(spaceshipSearchFilter);
            }

            bool isValid = (matches != null && matches.Count > 0);

            ValidateResult validateResult = new ValidateResult
            {
                IsValid = (isValid && value != null),
                Value = value
            };

            string feedbackMessage = string.Empty;

            if (!isValid)
            {
                // Since this was an invalid option, undo the change
                spaceshipState.ClearPropertyValue(propertyName);

                string valueAsString = ValueToString(value);
                System.Diagnostics.Debug.WriteLine($"Value {valueAsString} for property {propertyName} is invalid");
                feedbackMessage = $"\"{valueAsString}\" is not a valid option";
            }
            else
            {
                // Store the search
                spaceshipData.LastSearchResults = matches;
            }

            if (matches != null && matches.Count > 5)
            {
                feedbackMessage = $"Still {matches.Count} options matching your criteria. Let's get some more details!";
            }

            if (!string.IsNullOrEmpty(feedbackMessage))
            {
                System.Diagnostics.Debug.WriteLine(feedbackMessage);
                validateResult.Feedback = feedbackMessage;
            }

            return validateResult;
        }
#pragma warning restore 1998

        /// <summary>
        /// Checks whether the given field should be active or not.
        /// </summary>
        /// <param name="state">The current state i.e. details for the watch filled so far.</param>
        /// <param name="propertyName">The name of the field to check.</param>
        /// <returns>True, if the given field should be active (has values). False otherwise.</returns>
        private static bool SetFieldActive(Spaceship spaceshipState, string propertyName)
        {
            bool setActive = true;
            SpaceshipData spaceshipData = SpaceshipData.Instance;

            if (spaceshipData.LastSearchResults != null)
            {
                if (spaceshipData.LastSearchResults.Count < 2)
                {
                    // There's only one or no options left - it makes no sense to ask anymore questions
                    setActive = false;
                }

                if (OptionsLeftForProperty(spaceshipData.LastSearchResults, propertyName).Count < 2)
                {
                    // There's only one or no options left for the given property - again this question is pointless
                    setActive = false;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Failed to check if not to set the field active for property {propertyName} due to missing search results");
            }

            System.Diagnostics.Debug.WriteLine($"Set field active for property {propertyName}: {setActive}");
            return setActive;
        }

#pragma warning disable 1998
        /// <summary>
        /// Sets the values for the given field.
        /// </summary>
        /// <param name="spaceshipState">The current state i.e. details for the watch filled so far.</param>
        /// <param name="propertyName">The name of the property (field).</param>
        /// <param name="field">The field to populate.</param>
        /// <returns>True, if values found and field populated. False otherwise.</returns>
        private static async Task<bool> SetOptionsForFieldsAsync(
            Spaceship spaceshipState, string propertyName, Field<Spaceship> field)
        {
            bool valuesSet = false;
            SpaceshipData spaceshipData = SpaceshipData.Instance;

            if (spaceshipData.LastSearchResults != null)
            {
                // Clear the values to avoid duplicates since this method can be called many times
                field.RemoveValues();

                IList<object> values = OptionsLeftForProperty(spaceshipData.LastSearchResults, propertyName);

                foreach (object value in values)
                {
                    System.Diagnostics.Debug.WriteLine($"Adding value {value} for property {propertyName} to field {field.Name}");
                    string valueInTitleCase = CamelCaseToTitleCase(value.ToString());

                    field
                        .AddDescription(value, valueInTitleCase)
                        .AddTerms(value, valueInTitleCase);

                    valuesSet = true;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Failed to set values for property {propertyName} due to missing search results");
            }

            return valuesSet;
        }
#pragma warning restore 1998

#pragma warning disable 1998
        private static async Task<PromptAttribute> GenerateConfirmMessageAsync(Spaceship spaceshipState)
        {
            return new PromptAttribute("Is this correct: " + spaceshipState.ToString() + "? {||}")
            {
                ChoiceStyle = ChoiceStyleOptions.Default
            };
        }
#pragma warning restore 1998

        /// <summary>
        /// Checks the options for the given property left in the given list of spaceships.
        /// </summary>
        /// <param name="spaceshipsLeft">A list of spaceships to check the options from.</param>
        /// <param name="propertyName">The name of the property whose options to check.</param>
        /// <returns>The options (values for property) as a list.</returns>
        private static IList<object> OptionsLeftForProperty(IList<Spaceship> spaceshipsLeft, string propertyName)
        {
            IList<object> options = new List<object>();

            foreach (Spaceship spaceship in spaceshipsLeft)
            {
                if (propertyName.Equals(nameof(Spaceship.Size)))
                {
                    if (spaceship.Size != Spaceship.Sizes.NotDefined
                        && !options.Contains(spaceship.Size.ToString()))
                    {
                        options.Add(spaceship.Size);
                    }
                }
                else if (propertyName.Equals(nameof(Spaceship.Engines)))
                {
                    foreach (Spaceship.EngineTypes engineType in spaceship.Engines)
                    {
                        if (engineType != Spaceship.EngineTypes.NotDefined
                            && !options.Contains(engineType.ToString()))
                        {
                            options.Add(engineType);
                        }
                    }
                }
                else if (propertyName.Equals(nameof(Spaceship.Weapons)))
                {
                    foreach (Spaceship.WeaponTypes weaponType in spaceship.Weapons)
                    {
                        if (weaponType != Spaceship.WeaponTypes.NotDefined
                            && !options.Contains(weaponType.ToString()))
                        {
                            options.Add(weaponType);
                        }
                    }
                }
                else if (propertyName.Equals(nameof(Spaceship.Crew)))
                {
                    if (spaceship.Crew != Spaceship.CrewTypes.NotDefined
                        && !options.Contains(spaceship.Crew.ToString()))
                    {
                        options.Add(spaceship.Crew);
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"{options.Count} options left for property {propertyName}");
            return options;
        }

        /// <summary>
        /// Formats the given camel case string to title case string.
        /// Example: "TheseAreFourWords" => "These Are Four Words".
        /// </summary>
        /// <param name="camelCaseString">The string to format.</param>
        /// <returns>A formatted string.</returns>
        private static string CamelCaseToTitleCase(string camelCaseString)
        {
            string formatted = string.Empty;

            if (!string.IsNullOrEmpty(camelCaseString))
            {
                int lastStartIndex = 0;
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                for (int i = 1; i < camelCaseString.Length; ++i)
                {
                    bool isLastChar = (i == (camelCaseString.Length - 1));

                    if (char.IsUpper(camelCaseString[i]))
                    {
                        formatted += textInfo.ToTitleCase(camelCaseString.Substring(lastStartIndex, (i - lastStartIndex))) + " ";
                        lastStartIndex = i;

                        if (isLastChar)
                        {
                            // Last char is upper - given string could be e.g. "SlaveI" - so simply add the last char
                            formatted += camelCaseString[i];
                        }
                    }
                    else if (isLastChar)
                    {
                        formatted += textInfo.ToTitleCase(camelCaseString.Substring(lastStartIndex, (i - lastStartIndex + 1)));
                    }
                }
            }

            return formatted.Trim();
        }

        /// <summary>
        /// Helper method for displaying selected value(s) as string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ValueToString(object value)
        {
            if (value is List<object>)
            {
                string valuesAsString = string.Empty;
                List<object> values = (value as List<object>);

                for (int i = 0; i < values.Count; ++i)
                {
                    valuesAsString += values[i].ToString();

                    if (i < values.Count - 1)
                    {
                        valuesAsString += ", ";
                    }
                }

                return valuesAsString;
            }

            return value.ToString();
        }
    }
}