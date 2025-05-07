from socket import (socket, getaddrinfo)
from shared import (Logger)
from drivers import (RP2040)
import _thread
from machine import (Pin)
import time


class SocketHost:

    __logger: Logger
    __device: RP2040
    _sock: socket
    _interrupt: bool
    _led: Pin

    def __init__(self, logger: Logger, device: RP2040):
        self.__logger = logger
        self.__device = device
        self._sock = socket()
        self._interrupt = False
        self._led = Pin(17, Pin.OUT)

    def start_host(self):
        self._interrupt = False
        addr = getaddrinfo('0.0.0.0', 8000)[0][-1]
        self._sock.bind(addr)
        self._sock.listen(1)
        _thread.stack_size(8*1024)  # Necessary to account for the lack of call-stack depth in some cases on secondary threads
        _thread.start_new_thread(self._listen, [])

    def _listen(self):
        self.__logger.info("Started socket listener")
        while not self._interrupt:
            try:
                cl, addr = self._sock.accept()
                print("client connected from %s", (addr))
                self._led.value(1)
                request = str(cl.recv(1024))
                print(request)
                if request.find('/temp') >= 0:
                    response = self.__device.read_temperature()
                    cl.send(str(response))
                    cl.close()
                    time.sleep_ms(100)
                    self._led.value(0)
            except OSError as e:
                self.__logger.error(e)
                cl.close()
        
        cl.close()
        self.__logger.info("Stopped listening")
    
    def stop_host(self):
        self._interrupt = True