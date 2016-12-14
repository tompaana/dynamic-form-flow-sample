# Dynamic FormFlow Sample #

This sample demonstrates how to add dynamic features to FormFlow implementation
of a bot built on Microsoft Bot Framework. The central points are explained in
my blog post creatively named
[How to Create Dynamic FormFlow](http://tomipaananen.azurewebsites.net/?p=1641).

The approach implemented by this sample is especially useful for scenarios where
the user is searching an item from an existing catalog. Instead of displaying
all the options for narrowing down the desired item, we only display options
available based on the data previously collected. In some cases we can skip part
of the queries altogether as they have but only one option. See the three
example flows below.

**Few examples of the flow:**

| Flow 1 | Flow 2 | Flow 3 |
| ------ | ------ | ------ |
| ![Flow 1](Documentation/Screenshots/Scenario1.png?raw=true) | ![Flow 2](Documentation/Screenshots/Scenario2.png?raw=true) | ![Flow3](Documentation/Screenshots/Scenario3.png?raw=true) |


## Important classes ##

**[Spaceship](/DynamicFormFlowSample/Models/Spaceship.cs)** represents the item
we are searching for and its properties. This is the class the FormFlow takes
and starts filling values with.

**[SpaceshipData](/DynamicFormFlowSample/Data/SpaceshipData.cs)** contains the
catalog and some helper methods.

**[SpaceshipSelectionForm](/DynamicFormFlowSample/Forms/SpaceshipSelectionForm.cs)**
contains the FormFlow builder and the methods implementing the dynamics:
checking if the fields (queries) are necessary, what options should be available
and response validation.

**[SpaceshipSelectionDialog](/DynamicFormFlowSample/Dialogs/SpaceshipSelectionDialog.cs)**
is the root dialog of the bot and takes control of the flow once the FormFlow
is finished. If more than one option remains, it will display a carousel of the
spaceships left for the user to choose from.


## See also ##

* [FormFlow documentation](https://docs.botframework.com/en-us/csharp/builder/sdkreference/forms.html)
* [Pizza Bot Sample](https://github.com/Microsoft/BotBuilder/tree/master/CSharp/Samples/PizzaBot)
