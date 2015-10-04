lmci-uploader
==============

c# app for uploading screenshots.

Current (weak?) implementation runs invisible with no config. Ctrl+shift+hotkey where hotkey is:
* 1 - Take screenshot of monitor mouse is on
* 2 - Take screenshot of foreground window
* 3 - Take screenshot of all monitors
* 4 - Click and drag to select an area
Will then try to use an API key specified in a json file (currently) to upload to my server (current dev host), and will probably die horribly in any kind of failure scenario

originally based on work by [alex zhang](https://github.com/Zhangerr)

follow up from this one time i hacked an [existing](https://github.com/aevv/aevv_puush) [screenshotting client](https://github.com/aevv/puush_server)

Todo
----

* Backend API (dev version in php)
* Storage (psql blobs in text columns, thumbnail generation deferred?)
* Front end website
* Client UI
* Bulletproofing client/server
* Misc usability things
* SECURITY
