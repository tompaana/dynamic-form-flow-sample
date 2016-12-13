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

#pragma warning disable 1998
        public async Task StartAsync(IDialogContext context)
        {
            _spaceshipMatches = null;
            SpaceshipData.Instance.LastSearchResults = null;

            var spaceshipSelectionForm = new FormDialog<Spaceship>(new Spaceship(), BuildSpaceshipSelectionForm, FormOptions.None);
            context.Call(spaceshipSelectionForm, OnSpaceshipSelectionFormCompleteAsync);
        }
#pragma warning restore 1998

        /// <summary>
        /// Called once the form is complete. The result will have the properties selected by the user.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
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
                System.Diagnostics.Debug.WriteLine($"We've got the criteria for the ship:\n{spaceship.PropertiesAsFormattedString()}");
                SpaceshipData spaceshipData = SpaceshipData.Instance;
                _spaceshipMatches = spaceshipData.SearchForPartialMatches(spaceship);

                if (_spaceshipMatches.Count == 1)
                {
                    // Single match for the given property -> Choise is made!
                    IMessageActivity messageActivity = context.MakeMessage();
                    ThumbnailCard thumbnailCard = CreateSpaceshipThumbnailCard(_spaceshipMatches[0]);
                    messageActivity.Attachments = new List<Attachment>() { thumbnailCard.ToAttachment() };
                    messageActivity.Text = $"You've chosen \"{_spaceshipMatches[0].Name}\", well done!";
                    await context.PostAsync(messageActivity);
                    context.Done(_spaceshipMatches[0]);
                }
                else if (_spaceshipMatches.Count > 1)
                {
                    // More than one match -> Show the available options
                    await ShowSelectSpaceshipPromptAsync(context);
                }
                else
                {
                    // Nothing found matching the given criteria
                    await context.PostAsync("No spaceships found with the given criteria");
                    context.Fail(new ArgumentException("No spaceships found with the given criteria"));
                }
            }
            else
            {
                context.Fail(new NullReferenceException("No spaceship :("));
            }
        }

        /// <summary>
        /// Called when the user selects a spaceship from the shown options.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
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
                    IMessageActivity messageActivity = context.MakeMessage();
                    ThumbnailCard thumbnailCard = CreateSpaceshipThumbnailCard(selectedSpaceship);
                    messageActivity.Attachments = new List<Attachment>() { thumbnailCard.ToAttachment() };
                    messageActivity.Text = $"\"{spaceshipName}\" it is, great choice!";
                    await context.PostAsync(messageActivity);
                    context.Done(selectedSpaceship);
                }
                else
                {
                    await ShowSelectSpaceshipPromptAsync(context);
                }
            }
        }

        /// <summary>
        /// Displays the available spaceship options based on the search done after the form was
        /// completed.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
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
        /// Creates a thumbnail card for the given spaceship.
        /// </summary>
        /// <param name="spaceship"></param>
        /// <returns>A newly created thumbnail card.</returns>
        private ThumbnailCard CreateSpaceshipThumbnailCard(Spaceship spaceship)
        {
            ThumbnailCard thumbnailCard = null;

            if (spaceship != null)
            {
                thumbnailCard = new ThumbnailCard()
                {
                    Title = spaceship.Name,
                    Images = new[] { new CardImage(spaceship.ImageUri) },
                    Text = spaceship.PropertiesAsFormattedString()
                };
            }

            return thumbnailCard;
        }

        /// <summary>
        /// Creates a carousel with the given spaceships and inserts that into the given message activity.
        /// </summary>
        /// <param name="messageActivity">The message activity to insert the carousel into.</param>
        private void CreateSpaceshipCarousel(ref IMessageActivity messageActivity, IList<Spaceship> spaceships)
        {
            IList<ThumbnailCard> thumbnailCards = new List<ThumbnailCard>();

            foreach (Spaceship spaceship in spaceships)
            {
                ThumbnailCard thumbnailCard = CreateSpaceshipThumbnailCard(spaceship);
                thumbnailCard.Buttons =
                    new[] { new CardAction(type: ActionTypes.PostBack, title: "Select", value: spaceship.Name) };
                thumbnailCards.Add(thumbnailCard);
            }

            messageActivity.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            messageActivity.Attachments = thumbnailCards.Select(thumbnailCard => thumbnailCard.ToAttachment()).ToList();
            messageActivity.Text = "Spaceships matching your criteria";
        }
    }
}