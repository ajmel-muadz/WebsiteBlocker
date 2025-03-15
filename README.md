# WebsiteBlocker

This is a simple website blocker built with Windows Presentation Foundations (.NET) for
Windows computers. The application is still a work in progress.

## IMPORTANT NOTICE

This is the first proper program I made in .NET WPF. I did not implement this project in a
clean manner and I now realised I should have utilised an MVVM architecture. Although the
final project is basically done, I would like to keep working on this and eventually
refactor the code.

Another main feature I am planning to add is a timer feature for block sessions. That can only be
done once I refactor the codebase.

## How to install

This program is built for Microsoft Windows x64 (64-bit) machines. I think it also works
on 32-bit machines, but I cannot find one to test for.

To install the app, simply go to the sidebar on this page and download the [latest release.](https://github.com/ajmel-muadz/WebsiteBlocker/releases/tag/WebsiteBlocker%2Fv1.0)
Ensure you click on 'WebsiteBlocker.exe'.

Please note that this app also requires administrator privileges to run.

## How to use the program

The program requires adding website links to block. However, the links required to be added have to be precise. This is because the program
modifies [Windows hosts file](https://en.wikipedia.org/wiki/Hosts_(file)) to block websites. Let's take the following example site, [facebook.com](https://www.facebook.com/)

Upon entering the site, notice the URL:
![image](https://github.com/user-attachments/assets/fe83c7b3-a2e7-4e0f-8107-51f4fafe9191)

The full URL is 'https://www.facebook.com'.

Entering this into the website blocker program would not work. Why? Because Windows hosts files do not parse the 'https://' part of the URL. It only parses
'www.facebook.com'. Yes, it's that exact. This also means 'facebook.com' will also not work, only 'www.facebook.com'.

Also, sometimes websites are stored in the browser cache. If a block session has started but a website is still not blocked, **close the browser and restart**.
This should then block the website, as the browser cache is cleared.
