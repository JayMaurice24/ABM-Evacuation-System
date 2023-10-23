# MARS grid-based Model Starter

This model includes some basic building blocks for designing grid-based models in MARS. The environment of a grid-based model consists of a two-dimensional grid. Agents can move on the grid and interact with information in the grid cells as well as with each other.

## Project structure

Below is a brief description of each component of the project structure:

- `Program.cs`: the entry point of the model from which the model can be started. See [Model setup and execution](#model-setup-and-execution) below for more details.
- `config.json`: a JavaScript Object Notation (JSON) with which the model can be configured. See [Model configuration](#model-configuration) below for more details.
- `Model`: a directory that holds the agent types, layer types, entity types, and other classes of the model. See [Model description](#model-description) for more details.
- `Resources`: a directory that holds the external resources of the model that are integrated into the model at runtime. This includes initialization and parameterization files for agent types and layer types.


## Model setup and execution

The following tools are required on your machine to run a full simulation and visualization of this model:

- A C# Interactive Development Environment (IDE), preferably [JetBrains Rider](https://www.jetbrains.com/rider/)
- [.NET Core](https://dotnet.microsoft.com/en-us/download) 6.0 or higher
- [Python](https://www.python.org/downloads/) 3.8 or higher

To set up and run the simulation and visualization, please follow these steps:

1. Download or clone this repository
2. Navigate into the folder of this `README.md` in your terminal
3. Follow these instructions to start the visualization tool (alternatively, see the README in the `mars-live-viz` directory):
    1. Open a terminal in the `mars-live-viz` directory
    2. Execute the following command:
        1. macOS: `pip3 install -r requirements.txt`
        2. Windows: `pip install -r requirements.txt`
    3. Once the installation has finished, execute the following command:
        1. macOS: `python3 main.py`
        2. Windows: `python main.py`
    4. A black PyGame window should open. **Note:** Do not close the terminal.
    5. Alternatively to the above, the visualization tool can be started with a Python IDE such as [JetBrains PyCharm](https://www.jetbrains.com/pycharm/).
4. Open JetBrains Rider.
5. Open the solution file `WorkshopGrid/WorkshopGrid.sln`.
6. Run the `Main()` method in the file `Program.cs`.
7. The simulation should run in Rider and, simultaneously, a visualization should be displayed in the PyGame window.

# MARS Simple Visualization

A simple visualization to consume the real-time simulation output of a MARS model with it's WebSocket output.

You can use it to visualize Grid-Based models in real-time.

## Installation

- Open your terminal and navigate into this directory. 
- Install [Python](https://www.python.org/downloads/).
- Install dependencies with `$ pip install -r requirements.txt`

## Start

Start the mini visualization by calling `$ python main.py`. The visualization now waits for a MARS simulation to run.


## Interacting with the visualization

While the visualization is running, its speed (framerate) can be changed by pressing the up arrow (increase speed) or down arrow (decrease speed) on your keyboard.

PS: The simulation has to be stopped manually once the number of agents reach 0 