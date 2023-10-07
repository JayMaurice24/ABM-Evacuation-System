import pygame
import sys
import json
import csv
from websocket import create_connection, WebSocketConnectionClosedException

# Define constants for your simulation
GRID_SIZE = 20
GRID_WIDTH = 800
GRID_HEIGHT = 600
WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
ORANGE = (255, 165, 0)
GREY = (128, 128, 128)

# Load agent types from config.json
with open('config.json', 'r') as config_file:
    config_data = json.load(config_file)
    agent_types = {agent['name']: agent.get('count', 0) for agent in config_data['agents']}

# Load floor plan from CSV file
with open('Resources/HTF.csv', 'r') as csv_file:
    floor_plan = list(csv.reader(csv_file))

# Initialize Pygame
pygame.init()

# Create the screen
screen = pygame.display.set_mode((GRID_WIDTH, GRID_HEIGHT))
pygame.display.set_caption("MARS Grid-Based Simulation")

# Define a function to draw the grid
def draw_grid():
    for x in range(0, GRID_WIDTH, GRID_SIZE):
        pygame.draw.line(screen, BLACK, (x, 0), (x, GRID_HEIGHT))
    for y in range(0, GRID_HEIGHT, GRID_SIZE):
        pygame.draw.line(screen, BLACK, (0, y), (GRID_WIDTH, y))

# Define a function to draw agents
def draw_agents():
    for row_index, row in enumerate(floor_plan):
        for col_index, cell in enumerate(row):
            x = col_index * GRID_SIZE
            y = row_index * GRID_SIZE

            if cell == '1':
                # Draw a polygon for fire
                pygame.draw.polygon(screen, ORANGE, [(x, y), (x + GRID_SIZE, y), (x + GRID_SIZE, y + GRID_SIZE), (x, y + GRID_SIZE)])
            elif cell == '2':
                # Draw a polygon for smoke
                pygame.draw.polygon(screen, GREY, [(x, y), (x + GRID_SIZE, y), (x + GRID_SIZE, y + GRID_SIZE), (x, y + GRID_SIZE)])
            elif cell.isdigit():
                agent_type = int(cell)
                agent_count = agent_types.get(f"EvacueeType{agent_type}", 0)
                if agent_count > 0:
                    # Draw a circle for the agent with their type number
                    pygame.draw.circle(screen, BLACK, (x + GRID_SIZE // 2, y + GRID_SIZE // 2), GRID_SIZE // 3)
                    font = pygame.font.Font(None, 36)
                    text = font.render(str(agent_type), True, WHITE)
                    text_rect = text.get_rect(center=(x + GRID_SIZE // 2, y + GRID_SIZE // 2))
                    screen.blit(text, text_rect)
                    
def get_socket(self):
        ws = None
        while ws is None:
            try:
                ws = create_connection(self.uri)
                print("Connecting to simulation ...")
            except (ConnectionResetError, ConnectionRefusedError, TimeoutError, WebSocketConnectionClosedException):
                print("Waiting for running simulation ... ")
                time.sleep(2)
                ws = None
        return ws


# Main simulation loop
running = True
while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

    # Clear the screen
    screen.fill(WHITE)

    # Draw the grid
    draw_grid()

    # Draw the agents
    draw_agents()

    # Update the display
    pygame.display.flip()

# Quit Pygame
pygame.quit()
sys.exit()
