# SimpleWeatherApp
A simple Windows Form app to show Weather of any city like Google Weather. Minimizable to tray.

This is an attemp to use the online weather APIs to query weather informationa and use it in a weather app
This is a simple Windows Form app created in .Net. It shows the weather over a period of 7 days into the future and any number of days into the past. 
You can set the name of a place  in the edit box and refresh the  button to set a location.

The App uses World Weather Online APIs (http://developer.worldweatheronline.com/default.aspx) to query the information. You need to visit the above path and Sign Up to create a API key. The API Key needs to be entered in BasicUI/app.config under "WWOKey".

Alternatively you can also use APIs from OpenWeatherMap (http://openweathermap.org/api) to create a key in which case it needs to be entered under "OWAKey".


