import sys
import machine
import time
sys.path.append('shared/')
sys.path.append('net/')
sys.path.append('monitoring/')
sys.path.append('drivers/')
from shared import (FileIOProvider, Logger, ConfigProvider)
from drivers import (RP2040)
from net import (WifiConnection)
from monitoring import (SocketHost)

logger = Logger('Main')
provider = FileIOProvider(logger)
config = ConfigProvider(logger, provider)
connection = WifiConnection(logger, config)
device = RP2040(True, True)
monitor = SocketHost(logger, device)

device.turn_led_on()

config.read('wifi_user')
print(config._settings)

connection.connect()

print(connection.connection_info())

try:
    monitor.start_host()
    while True:
        time.sleep(1)
except KeyboardInterrupt:
    monitor.stop_host()
    device.turn_led_off()
    machine.reset()
    sys.exit(1)


"""
pin = Pin("LED", Pin.OUT)

print("LED starts flashing...")
while True:
    try:
        pin.toggle()
        sleep(1) # sleep 1sec
    except KeyboardInterrupt:
        break
pin.off()
print("Finished.")
"""