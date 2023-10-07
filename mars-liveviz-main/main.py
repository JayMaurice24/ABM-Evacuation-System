import _thread as thread
import csv
import json
import time
import upsidedown 
import pygame
from pygame import RESIZABLE, DOUBLEBUF, HWSURFACE
from websocket import create_connection, WebSocketConnectionClosedException

import lock

GRAY = (100, 100, 100)
NAVYBLUE = (60, 60, 100)
WHITE = (255, 255, 255)
RED = (255, 0, 0)
GREEN = (0, 255, 0)
BLUE = (0, 0, 255)
YELLOW = (255, 255, 0)
ORANGE = (255, 128, 0)
PURPLE = (255, 0, 255)
CYAN = (0, 255, 255)
BLACK = (0, 0, 0)

GREEN_LIGHT = [149, 217, 104, 10]
YELLOW_LIGHT = [205, 217, 104, 10]
PURPLE_LIGHT = [162, 134, 222, 10]
BLUE_LIGHT = [174, 218, 233, 10]

RASTER_COLORS = {
    1: GREEN_LIGHT,
    2: YELLOW_LIGHT,
    3: PURPLE_LIGHT,
    4: BLUE_LIGHT,
}
UNKNOWN_RASTER = WHITE
AGENT_COLORS = [GREEN, YELLOW, RED]

# WINDOW_SIZE = 800, 800

class Visualization:
    def __init__(self):
        pygame.init()
        pygame.display.set_caption("MARS-Mini-VIS")

        self.programIcon = pygame.image.load('icon.png')
        pygame.display.set_icon(self.programIcon)

        self.clock = pygame.time.Clock()
        self.WINDOW_SIZE = [1200, 880]
        self.WORLD_SIZE = 0, 0, 119, 87  # used for scaling
        self.BORDER_WIDTH_PIXEL = -20
        self.font = pygame.font.Font('freesansbold.ttf', 12)
        self.text = self.font.render('Tick: 0', True, YELLOW)
        self.fps_text = self.font.render('FPS: 0', True, YELLOW)
        self.desired_fps = self.font.render('Desired FPS: 0', True, YELLOW)
        self.textRect = self.text.get_rect()
        self.fpsTextRect = self.fps_text.get_rect()
        self.desired_fpsRect = self.desired_fps.get_rect()
        
        self.textLoading = self.font.render('Waiting for MARS simulation to start...', True, WHITE)

        self.l = lock.RWLock()
        self.pressed_up = False
        self.pressed_down = False
        self.entities = {}
        self.point_features = []
        self.line_features = []
        self.ring_features = []
        self.polygon_features = []
        self.raster_metadata = []
        self.tick_display = [False, 0, 1000]
        self.fps = 360
        self.run = True
        self.uri = "ws://127.0.0.1:4567/vis"
        self.ws = None
        self.desired_fps = self.fps
        self.time_to_wait_milliseconds = 10
        self.borderColor = (255, 255, 255)
        self.barColor = (0, 128, 0)
        self.has_welcome_been_printed = False

        self.set_window_relations(self.WINDOW_SIZE[0], self.WINDOW_SIZE[1])

    def set_window_relations(self, width, height):
        self.WINDOW_SIZE[0] = width
        self.WINDOW_SIZE[1] = height
        self.textRect.center = (30, height - 12)
        self.fpsTextRect.center = (120, height - 12)
        self.desired_fpsRect.center = (230, height - 12)
        self.barPos = (10, height - 40)
        self.barSize = (self.WINDOW_SIZE[0] - 20, 20)
        self.screen = pygame.display.set_mode((self.WINDOW_SIZE[0], height), HWSURFACE | DOUBLEBUF | RESIZABLE)

        if not self.has_welcome_been_printed:
            self.screen.fill(GRAY)
            textLoadingRect = self.textLoading.get_rect(center=(self.WINDOW_SIZE[0]/2, self.WINDOW_SIZE[1]/2))
            self.screen.blit(self.textLoading, textLoadingRect)
            pygame.display.update()
            self.has_welcome_been_printed = True

        self.gamePos = (10, 10)
        self.gameSize = (self.WINDOW_SIZE[0] - 20, self.WINDOW_SIZE[1] - 60)

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

    def load_data(self):
        if self.ws is None:
            self.ws = self.get_socket()
        try:
            message = self.ws.recv()

            if len(message) == 0 or message is None:
                print("Could not receive data, is the simulation still running?")
                self.ws = None
                return

            # print(message)
            data = json.loads(message)

            self.l.acquire_write()
            if "currentTick" in data:
                self.tick_display[1] = data["currentTick"]
            if "maxTicks" in data:
                self.tick_display[2] = data["maxTicks"]

            if "entities" in data:
                entities_points = data["entities"]
                self.entities[data['t']] = entities_points
            if "worldSize" in data:
                world_data = data["worldSize"]
                if world_data["maxX"] > 0:
                    self.WORLD_SIZE = world_data["minX"], world_data["minY"], world_data["maxX"], world_data["maxY"]
            with open('HTF.csv', 'r') as csv_file:
                raster_data= csv.reader(csv_file, delimiter=';')
                for raster in raster_data:
                    self.raster_metadata.append(raster)

            self.tick_display[0] = True
            self.l.release_write()
        except (ConnectionResetError, ConnectionRefusedError, WebSocketConnectionClosedException):
            self.ws.shutdown()
            self.entities.clear()
            if self.l.get_active_writer() > 0:
                self.l.release_write()
            self.screen.fill(BLACK)
            self.ws = None

    def visualize_content(self):
        self.clock.tick(self.desired_fps)
        self.load_data()
       
        if not self.tick_display[0]:
            return

        self.screen.fill(GRAY)

        # Size of the raster area, WORLD_SIZE contains minX/minY/maxX/maxY, to get
        # the board size we use maxX - minX + 1 (i.e. 9 - 0 + 1 => 10 cells).
        delta_x = self.WORLD_SIZE[2] - self.WORLD_SIZE[0] + 1
        delta_y = self.WORLD_SIZE[3] - self.WORLD_SIZE[1] + 1

        # game size is the area for viz of the simulation area, the size divided by the amount of cells
        # gives us the width and height of a single cell.
        scale_x = self.gameSize[0] / delta_x
        scale_y = self.gameSize[1] / delta_y

        # line_width = int((scale_x + scale_y) / 2)
        line_width = 1
        agent_size = 7

        surface = pygame.Surface(self.gameSize)

        # draw grid on everything
        for x in range(1, delta_x):
            for y in range(1, delta_y):
                pygame.draw.line(surface, BLUE_LIGHT, (x * scale_x, 0), (x * scale_x, self.gameSize[1]))
                pygame.draw.line(surface, BLUE_LIGHT, (0, y * scale_y), (self.gameSize[0], y * scale_y))

        for row_index, row in enumerate(self.raster_metadata):
            for col_index, cell_value in enumerate(row):
                x = col_index * scale_x  # Adjust this based on cell size
                y = (len(self.raster_metadata) - row_index - 1) * scale_y  # Adjust this based on cell size
                width = 1 * scale_x  # Adjust this based on cell size
                height = 1 * scale_y  # Adjust this based on cell size

                # Determine color based on cell_value
                color = UNKNOWN_RASTER
                color = BLACK
                if cell_value == '1':
                    color = BLUE_LIGHT

                pygame.draw.rect(surface, color, pygame.Rect(x, y, width, height))

        for type_key in self.entities.keys():
            for entity in self.entities[type_key]:
                x = entity["x"]
                y = entity["y"]

                if type_key == 19:
                    pygame.draw.rect(surface, ORANGE,
                             pygame.Rect((x - self.WORLD_SIZE[0]) * scale_x, (y - self.WORLD_SIZE[1]) * scale_y,
                                         scale_x, scale_y))
                elif type_key == 20:
                    pygame.draw.line(surface, BLUE_LIGHT,
                             ((x - self.WORLD_SIZE[0]) * scale_x, (y - self.WORLD_SIZE[1]) * scale_y),
                             ((x - self.WORLD_SIZE[0]) * scale_x + scale_x, (y - self.WORLD_SIZE[1]) * scale_y + scale_y),
                             line_width)
                    pygame.draw.line(surface, AGENT_COLORS[type_key % len(AGENT_COLORS)],
                             ((x - self.WORLD_SIZE[0]) * scale_x + scale_x, (y - self.WORLD_SIZE[1]) * scale_y),
                             ((x - self.WORLD_SIZE[0]) * scale_x, (y - self.WORLD_SIZE[1]) * scale_y + scale_y),
                             line_width)
                elif type_key == 21:
                    pygame.draw.rect(surface, GRAY,
                             pygame.Rect((x - self.WORLD_SIZE[0]) * scale_x, (y - self.WORLD_SIZE[1]) * scale_y,
                                         scale_x, scale_y))
                else:
                    pygame.draw.circle(surface, AGENT_COLORS[type_key % len(AGENT_COLORS)],   (((x - self.WORLD_SIZE[0]) * scale_x + scale_x / 2),
                                    ((y - self.WORLD_SIZE[1]) * scale_y) + scale_y / 2),
                                   line_width * agent_size, 0)
                    font = pygame.font.Font(None, 15)
                    text_surface = font.render(((str(type_key))), True, BLACK)
                    text_rect = text_surface.get_rect(center=((x - self.WORLD_SIZE[0]) * scale_x + scale_x / 2,
                                                       (y - self.WORLD_SIZE[1]) * scale_y + scale_y / 2))
                    surface.blit(text_surface, text_rect)
        # Map game area to main view
        flipped = pygame.transform.flip(surface, False, True)
        self.screen.blit(flipped, (10, 10))
        pygame.draw.rect(self.screen, WHITE, (*self.gamePos, *self.gameSize), 1)

        # Update tick counter
        self.screen.blit(self.font.render(f'Tick: {self.tick_display[1]}', True, WHITE), self.textRect)
        self.screen.blit(self.font.render(f'FPS: {round(self.clock.get_fps(), 2)}', True, WHITE), self.fpsTextRect)
        self.screen.blit(
            self.font.render(f'Desired FPS: {self.desired_fps} (use up- and down arrows to change)', True, WHITE),
            self.desired_fpsRect)

        # Update progressbar
        if self.tick_display[2] != 0:
            progress = self.tick_display[1] / self.tick_display[2]
            self.draw_progress(self.barPos, self.barSize, self.borderColor, self.barColor, progress)

        pygame.display.update()

    def draw_progress(self, pos, size, border_c, bar_c, progress):
        pygame.draw.rect(self.screen, BLACK, (*pos, *size))
        pygame.draw.rect(self.screen, border_c, (*pos, *size), 1)
        inner_pos = (pos[0] + 3, pos[1] + 3)
        inner_size = ((size[0] - 6) * progress, size[1] - 6)
        pygame.draw.rect(self.screen, bar_c, (*inner_pos, *inner_size))

    def handle_inputs(self):
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                print("Stop visualization ...")
                self.run = False
            if event.type == pygame.VIDEORESIZE:
                width, height = event.size
                if width < 500:
                    width = 500
                if height < 500:
                    height = 500
                self.set_window_relations(width, height)
            if event.type == pygame.KEYDOWN:
                if self.ws is not None:
                    if event.key == pygame.K_LEFT:
                        self.time_to_wait_milliseconds = (self.time_to_wait_milliseconds - 3)
                        if self.time_to_wait_milliseconds < 0:
                            self.time_to_wait_milliseconds = 3
                        self.ws.send(json.dumps({"timeToWaitInMilliseconds": self.time_to_wait_milliseconds}))
                    if event.key == pygame.K_RIGHT:
                        self.time_to_wait_milliseconds = (self.time_to_wait_milliseconds + 10)
                        self.ws.send(json.dumps({"timeToWaitInMilliseconds": self.time_to_wait_milliseconds}))
        keys = pygame.key.get_pressed()
        if keys[pygame.K_DOWN]:
            self.desired_fps = (self.desired_fps - 3)
            if self.desired_fps < 0:
                self.desired_fps = 5
        elif keys[pygame.K_UP]:
            self.desired_fps = (self.desired_fps + 3)
            if self.desired_fps >= 1000:
                self.desired_fps = 1000
        elif keys[pygame.K_q]:
            self.run = not self.run

    def content_loop(self):
        while self.run:
            self.visualize_content()

    def visualization_loop(self):

        thread.start_new_thread(self.content_loop, ())

        while self.run:
            self.clock.tick(self.desired_fps)
            self.handle_inputs()
            # self.visualize_content()

        pygame.quit()


vis = Visualization()
vis.visualization_loop()
