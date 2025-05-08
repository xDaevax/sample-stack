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
from monitoring import (SocketHost, InputMonitor)

interrupt: bool = False
logger = Logger('Main')
provider = FileIOProvider(logger)
config = ConfigProvider(logger, provider)
connection = WifiConnection(logger, config)
device = RP2040(True, True)
monitor = SocketHost(logger, device)
input = InputMonitor(logger, 16, interrupt)

config.read('wifi_user')
print(config._settings)

def connect():
    global connection
    global device
    connection.connect()
    device.turn_led_on()
    print(connection.connection_info())

def disconnect():
    global connection
    global device

    connection.disconnect()
    device.turn_led_off()

def reconnect():
    global device
    global connection
    disconnect()
    connect()


try:
    connect()
    monitor.start_host()
    input.listen(reconnect)
except KeyboardInterrupt:
    interrupt = True
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