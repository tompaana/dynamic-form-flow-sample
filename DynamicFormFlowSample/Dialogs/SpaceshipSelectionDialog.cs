using DynamicFormFlowSample.Models;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using DynamicFormFlowSample.Data;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using System.Linq;

namespace DynamicFormFlowSample.Dialogs
{
    [Serializable]
    public class SpaceshipSelectionDialog : IDialog<Spaceship>
    {
        private readonly BuildFormDelegate<Spaceship> BuildSpaceshipSelectionForm;
        private IList<Spaceship> _spaceshipMatches;

        internal SpaceshipSelectionDialog(BuildFormDelegate<Spaceship> buildSpaceshipSelectionForm)
        {
            BuildSpaceshipSelectionForm = buildSpaceshipSelectionForm;
        }

        public async Task StartAsync(IDialogContext context)
        {
            _spaceshipMatches = null;
            SpaceshipData.Instance.LastSearchResults = null;

            await context.PostAsync("Greetings!");
            var spaceshipSelectionForm = new FormDialog<Spaceship>(new Spaceship(), BuildSpaceshipSelectionForm, FormOptions.None);
            context.Call(spaceshipSelectionForm, OnSpaceshipSelectionFormCompleteAsync);
        }

        private async Task OnSpaceshipSelectionFormCompleteAsync(IDialogContext context, IAwaitable<Spaceship> result)
        {
            Spaceship spaceship = null;

            try
            {
                spaceship = await result;
            }
            catch (FormCanceledException e)
            {
                System.Diagnostics.Debug.WriteLine($"Form canceled: {e.Message}");
            }

            if (spaceship != null)
            {
                System.Diagnostics.Debug.WriteLine($"We've got the criteria for the ship: {spaceship.ToString()}");
                SpaceshipData spaceshipData = SpaceshipData.Instance;
                _spaceshipMatches = spaceshipData.SearchForPartialMatches(spaceship);

                if (_spaceshipMatches.Count == 1)
                {
                    await context.PostAsync($"You've chosen \"{_spaceshipMatches[0].Name}\", well done!");
                    context.Done(_spaceshipMatches[0]);
                }
                else if (_spaceshipMatches.Count > 1)
                {
                    await ShowSelectSpaceshipPromptAsync(context);
                }
                else
                {
                    await context.PostAsync("No spaceships found with the given criteria");
                    context.Fail(new ArgumentException("No spaceships found with the given criteria"));
                }
            }
            else
            {
                context.Fail(new NullReferenceException("No spaceship :("));
            }
        }

        private async Task OnSpaceshipSelectedAsync(IDialogContext context, IAwaitable<object> result)
        {
            object awaitedResult = await result;
            string spaceshipName = null;

            if (awaitedResult != null)
            {
                if (awaitedResult is Activity)
                {
                    spaceshipName = (awaitedResult as Activity).Text;
                }
                else
                {
                    spaceshipName = awaitedResult.ToString();
                }
            }

            if (!string.IsNullOrEmpty(spaceshipName))
            {
                SpaceshipData spaceshipData = SpaceshipData.Instance;
                Spaceship selectedSpaceship = null;

                try
                {
                    selectedSpaceship = spaceshipData.Spaceships.First(spaceship => spaceship.Name.Equals(spaceshipName));
                }
                catch (InvalidOperationException)
                {
                    await context.PostAsync($"\"{spaceshipName}\" is not a valid option, please try again");
                }

                if (selectedSpaceship != null)
                {
                    await context.PostAsync($"\"{spaceshipName}\" it is, great choice!");
                    context.Done(selectedSpaceship);
                }
                else
                {
                    await ShowSelectSpaceshipPromptAsync(context);
                }
            }
        }

        private async Task ShowSelectSpaceshipPromptAsync(IDialogContext context)
        {
            if (_spaceshipMatches != null)
            {
                if (_spaceshipMatches.Count > 1)
                {
                    IMessageActivity messageActivity = context.MakeMessage();
                    CreateSpaceshipCarousel(ref messageActivity, _spaceshipMatches);
                    await context.PostAsync(messageActivity);
                    context.Wait(OnSpaceshipSelectedAsync);
                }
                else
                {
                    context.Fail(new ArgumentException($"Need to have at least two spaceships ({nameof(_spaceshipMatches)})"));
                }
            }
            else
            {
                context.Fail(new NullReferenceException($"No list of matching spaceships ({nameof(_spaceshipMatches)})"));
            }
        }

        /// <summary>
        /// Creates a carousel with the given spaceships and inserts that into the given message activity.
        /// </summary>
        /// <param name="messageActivity">The message activity to insert the carousel into.</param>
        private void CreateSpaceshipCarousel(ref IMessageActivity messageActivity, IList<Spaceship> spaceships)
        {
            var cards = spaceships.Select(spaceship => new ThumbnailCard
            {
                Title = spaceship.Name,
                //Images = new[] { new CardImage(spaceship.ImageUrl) },
                Buttons = new[] { new CardAction(type: ActionTypes.PostBack, title: "Select", value: spaceship.Name) }
            });

            messageActivity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            messageActivity.Attachments = cards.Select(card => card.ToAttachment()).ToList();
            messageActivity.Text = "Spaceships matching your criteria";
        }
    }
}