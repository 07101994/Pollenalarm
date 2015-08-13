# Pollenalarm
Brace yourself, pollen are coming! Pollenalarm is an application that helps allergy sufferers to take their medicine and plan their trips according to the current pollen solution.

## Where does it work?
Unfortunately thsi only works for German cities at the moment but I am trying to get worldwide pollen information soon. Some sample cities in Germany are:
- Berlin (10115)
- Nuremberg (90409)
- Cologne (51069)
- Aachen (52080)

## Technology
### Backend
The server runs on ASP.NET technology and is responsible for crawling the pollution information, writing them into the database and providing them in form of JESON REST API.

### Frontend
The mobile applications are based on Xamarin technology. At the moment only an Android implementation exists but more are coming soon. They are all written in C# which makes it possible to share code between each other and the server.

### Shared code
The portable class library shares code between the server and the forntend projects. It includes the view models and some basic funtionality like downloading information from the server and calculating the pollen pollution.

### Testing
The frontend code is tested with the Xamarin.UITest framework and is based on NUnit. To fight device fragmentation and to see how it performs in the real world, Xamarin Test Cloud is used.





