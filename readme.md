# Horse Saddle Version 1.0.2

[![N|Solid](https://static.wixstatic.com/media/29dac0_84c639f416df456883d70bd8ecdae970~mv2.png/v1/fill/w_100,h_100,al_c,q_85,usm_0.66_1.00_0.01/LogoV2.webp)](https://www.lovebirb.com/)

This is a tool made to make turn-based matches of Totally Accurate Battle Simulator way less tedious by automating the maths involved.

# Quick Start

| Control     | Purpose                                |
|-------------|----------------------------------------|
| F11         | Toggles fullscreen.                    |
| Mouse wheel | Rotates the wheel by a single segment. |

Check out the Settings.xml file to change the names of the teams to match the players taking part. This can be edited with notepad or any other text editor.

Buttons on the bottom control the current wheel, buttons at the top switch the current wheel. The "Remove" button removes the current segment from the wheel, the "Reset" button reloads the wheel from file.

On the left and right there are the team displays. Next to the player's names are their scores, clicking on the pointed buttons next to the scores increases or decreases the score, and also adds base points to both teams (as the round has incremented). Clicking on the + button next to the "Cost" display allows you to add a free unit to that team, clicking the - button on a free unit removes it. The + and - buttons next to the "Bonus" display add and substract 250 points from that team's bonus.

The "Do Left/Right" and "Do" buttons appear when an action exists for the current segment. For the wheel of units, this adds a free unit to the left/right team, for the wheel of leaders, this adds the unit to both teams, the "Do" buttons also remove the segment for you.

Total Budget is the amount of points that team has in total, so that their points in TABS should not go over it. And with that, you're ready to spin! Check out the rest of this readme if you want to customise the program further.

# Settings File

The main settings node's attributes are as follows:
| Attribute Name | Type          | Purpose                                                                          |
|----------------|---------------|----------------------------------------------------------------------------------|
| ScoreName      | String (text) | The name of the score word to use. e.g. "HORSE".                                 |
| PointsPerRound | Number        | How many points are given to both teams per round.                               |
| WheelName      | File path     | The name of the folder under the "WheelPresets" folder to use as a wheel preset. |
| Background     | Colour        | The colour of the background.                                                    |

The team nodes' attributes are as follows:
| Attribute Name | Type          | Purpose                                      |
|----------------|---------------|----------------------------------------------|
| Name           | String (text) | The name of the player.                      |
| Colour         | Colour        | The colour of the player's team within TABS. |

# Wheel Presets

Wheel presets are found under the "WheelPresets" folder, each wheel preset gets its own folder.

Within this folder, a "Wheels.xml" file describes each wheel in the preset, and the "Units.xml" describes the units to be used by the preset.

## Wheels File

This xml file has a main node with a "Name" attribute, naming the entire preset. Under this node should exist multiple "Wheel" nodes, each one describing a wheel.

The attributes for a wheel are as follows:
| Attribute Name | Type          | Purpose                                                                 |
|----------------|---------------|-------------------------------------------------------------------------|
| Name           | String (text) | The wheel's name.                                                       |
| Radius         | Number        | The wheel's radius in pixels (mostly unused as the wheel auto-resizes). |
| InnerRadius    | Number        | The radius in pixels of the inner circle within the wheel.              |
| InnerColour    | Colour        | The colour of the inner circle within the wheel.                        |
| Colours        | Colours       | The comma-separated list of colours for the segments.                   |
| BorderColour   | Colour        | The colour of the border around the wheel and the inner circle.         |
| DividerColour  | Colour        | The colour of the line between segments.                                |
| TextPadding    | Number        | How many pixels from the edge of the circle the text starts.            |
| Drag           | Number        | The number of radians per second taken from the speed of the wheel.     |
| InnerImage     | File path     | The name of an image file to be displayed over the centre of the wheel. |
| IndicatorImage | File path     | The name of an image file used to indicate the current segment.         |
| IconImage      | File path     | The name of an image file used to represent the wheel in the icon bar.  |

There are two different child types that go under the wheel node, "Segment" and "Repeat". Repeat nodes just duplicate any segment nodes under them, adjustable with the "Amount" attribute. Segment nodes just have a "Name" which sets the name to be displayed on the wheel, and an "Action" attribute, which relates to the below actions segment.

## Actions

Actions can be assigned to segments in the Wheels.xml file under the wheel preset. If no action is given to a segment, then the "Do" buttons will be hidden. 

There are 4 actions assigned to segments:
| Name             | Purpose                                                          | Parameter 1                                                                                          | Parameter 2                                                                                   |
|------------------|------------------------------------------------------------------|------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------|
| AddFreeUnit    | Adds the free unit to the left/right team.                       | "Amount" - the amount of units to give, defaulting to 1.                                             | "Unit" - the name of the unit within the Units.xml file, defaults to the name of the segment. |
| AddLeader       | Adds the free unit to both teams.                                | "Unit" - the name of the leader unit within the Units.xml file, defaults to the name of the segment. |                                                                                               |
| AddBonusPoints | Adds bonus points to the left/right team.                        | "Amount" - the number of points to give.                                                             |                                                                                               |
| SwitchArmies    | Switches the free units, bonus points, and colours of the teams. |                                                                                                      |                                                                                               |

# Running the Program
## Controls

| Control     | Purpose                                |
|-------------|----------------------------------------|
| F11         | Toggles fullscreen.                    |
| Mouse wheel | Rotates the wheel by a single segment. |
## Bottom Buttons

At the bottom of the screen you should see some buttons relating to the wheel. Some are hidden and will only show up when an action can be made.

Spin - Spins the wheel.
Remove - Removes the current segment from the wheel.
Reset - Reloads the wheel from file.
Do - Does an action. This depends on the current segment's action.
Do Left/Right - Does an action to the left or right team, depending on the current segment.

## Top UI

At the top of the screen is the icon bar to select the different wheels, clicking on one of these buttons will switch to that wheel.

Below the icon bar is the name of the current wheel.

Below the wheel name is the current segment under the indicator.

Then of course, there is the wheel itself. 

## Team Displays

On the left and right side of the program are displays for each team. This shows the name, score, free units, and budget of either team.

To the right of the player's name exists the score display with a button on either side. The right button adds a letter of the score (e.g. if the word is "HORSE" then the first click of this button adds a "H") to that player and gives both players base points (this amount can be changed in the settings, but is 2500 by default).

Under the player's name is the free units list. The + button next to the "Cost" label opens a window that allows any unit in the game to be added to that player's free units. The - button on a free unit allows it to be removed.

At the bottom of the display is the budget of the player. It takes into account the total cost of the free units, any bonus points, and the base cost (the budget that goes up every round). The bonus budget has +/- buttons to add/remove 250 points from the bonus. This is all added up to give the total budget, which the player's army cost should not exceed.

# Credits

GuiCookie, LiruGameHelper, LiruGameHelperMonogame, and Horse Saddle by [Laura Jenkins (Liru)](https://www.lovebirb.com/)
Pixel UI by Kenney (www.kenney.nl)
Most icons are from TABS, supplied by the devs (along with usage permission). Other icons were hastily scribbled by Liru
Wheel contents from [Yogscast Tom](https://www.youtube.com/channel/UC5rUMdCFWPXYs9e8PBLzq5g)
[Dillinger](https://dillinger.io/) for the great markdown tool for this readme file