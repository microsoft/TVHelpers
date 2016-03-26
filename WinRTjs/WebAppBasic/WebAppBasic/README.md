This project does not include any third party script libraries.  You can test out loading a third-party script such as ReactJS by following these steps.

1. Download ReactJS from http://facebook.github.io/react/.
2. Copy the eactjs files to the \scriptlibrary folder
3. Copy base.css from the reactjs basic example to \scriptlibrary\base.css
3. Copy the reactjs basic sample html file to the project route and rename to index.html.
4. Modify index.html to include base.css and reactjs from the \scriptlibrary folder
5. Default.html will load index.html into an iframe:  <iframe id="ui" src="ms-appx-web:///index.html" aria-label="UI"></iframe>
6. Set a breakpoint in default.js and run the project.  
7. Once you hit a breakpoint, go to the Debug | Windows menu in Visual Studio 2015 and select JavaScript Console.
8. Type "window.Windows." in the JavaScript Console to see that you have access to the Windows Runtime API as a result of the <uap:Rule Match="ms-appx-web:///" Type="include" WindowsRuntimeAccess="all" /> line in the package.appxmanifest.
9. Note that you may need to open the package.appxmanifest file using the XML editor to inspect the configuration.

Please note, any third-party script library can be used to build your web app on Windows 10.  These steps just happens to demonstrate key concepts using ReactJS.