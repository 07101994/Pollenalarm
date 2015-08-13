# Pollenalarm
> Brace yourself, pollen are coming!

Pollenalarm is an application that helps allergy sufferers to take their medicine and plan their trips according to the current pollen solution.

![demo]
[demo]: https://raw.githubusercontent.com/Thepagedot/Pollenalarm/master/Misc/Demo.gif "App Demo"

## Where does it work?
Unfortunately this only works for German cities at the moment but I am trying to get worldwide pollen information soon. Some sample cities in Germany are:
- Berlin (10115)
- Nuremberg (90409)
- Cologne (51069)
- Aachen (52080)

## Technology
### Backend
The server runs on ASP.NET technology and is responsible for crawling the pollution information, writing them into the database and providing them in form of JSON REST API.

### Frontend
The mobile applications are based on [Xamarin](http://xamarin.com/) technology. At the moment only an Android implementation exists but more are coming soon. They are all written in C# which makes it possible to share code between each other and the server.

### Shared code
The portable class library shares code between the server and the forntend projects. It includes the view models and some basic funtionality like downloading information from the server and calculating the pollen pollution.

### Testing
The frontend code is tested with the Xamarin.UITest framework and is based on NUnit. To fight device fragmentation and to see how it performs in the real world, [Xamarin Test Cloud](http://xamarin.com/test-cloud) is used. [You can read here](http://wp.me/p6yt1c-DH) how this works and take at the look at the [UI Test cases](https://github.com/Thepagedot/Pollenalarm/blob/master/Pollenalarm.Android/Pollenalarm.Android.UITests/Tests.cs).

![test]
[test]: https://raw.githubusercontent.com/Thepagedot/Pollenalarm/master/Misc/TestResults.png "Test Results"

## License
The MIT License (MIT)

Copyright (c) 2015 Thepagedot

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.



