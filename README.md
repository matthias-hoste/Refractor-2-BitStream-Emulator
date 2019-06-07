# Refractor 2 BitStream Emulator

This program is capable of emulating the network traffic for a refractor 2 game. As an example I have added a base for Battlefield 2 emulation.
There is currently an issue where the Battlefield 2 client disconnects after we send the ServerInfo and MapList. I am unsure what the problem is currently.

It is a fully plugable system, you can add support for any refractor 2 game.
To add support you may be required to reverse certain functions to find certain parameters.

This project came to life after seeing how part of the bitstream worked in Aluigi's BF2Loop, the writebits/readbits functions are his, I just converted them to C#.
