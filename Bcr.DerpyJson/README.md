# Bcr.DerpyJson -- The dumbest C# JSON parser

## The Problem

I tried using `System.Text.Json.JsonSerializer` to parse JSON on the
[Meadow](https://www.wildernesslabs.co/) and it took ten seconds.

I didn't want to wait that long. This one takes less time.

## Disclaimers

### General

* The philosophy is to be ignorant of bad input. I'll probably make a
  reasonable attempt to handle bad input if it comes up. But I'm not
  actively looking for trouble.
* The top-level input is a `Span<byte>` so you better have it all in
  one place. It's possible that an `IEnumerator<byte>` is possible with a
  pushback mechanism for one symbol, but this may be at the cost of
  more temporary strings and allocations.

### Objects

* The way Values from an Object are handled is the parsed Value is set to
  the Property from the parent Type specified.

### Strings

* Backslash escapes are not supported. If there is a `\"` in a string, it
  won't notice that it's escaped and your string will end prematurely and
  will probably bomb out. Any other backslash escape will be included
  literally in the string.

### Numbers

* Numbers will probably explode impressively if you give them something
  stupid. I call the `Parse` method on the `Type` specified, and if
  you hand, say, a number with a decimal point in it but the type is
  `int` it will hurt.

### Booleans

* Not supported.

### null (this is JSON Value null, not a JavaScript error)

* Not supported.

### Arrays

* Not supported
