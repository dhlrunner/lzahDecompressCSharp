# lzahDecompressCSharp
C# implementation of de_lzah.c

## Usage
```CSharp
lzah lz = new lzah();
byte[] decompdata = lz.Decompress(<byte array of lzah compressed data>, <integer of Decompressed size>);

//Additional CRC16 calculate function
byte[] crc = lz.CalculateCRC16(<byte array of data buffer>);

```

## Credit
https://github.com/mistydemeo/quickbms/blob/master/compression/de_lzah.c
