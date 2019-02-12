# README #

This an application to test IDTech Augusta Devices.

### What is this repository for? ###

* JSON File reader utility
* 1.00.0.(100)
* git remote add origin git@github.com:web-projects/TC_IDTECH_CONFIG_READER.git

### How do I get set up? ###

* Summary of set up
* Configuration
* Dependencies
* Database configuration
* How to run tests
* Deployment instructions

### Contribution guidelines ###

* Writing tests
* Code review
* Other guidelines

### GIT NOTES ###

*  AUTO-CONVERTING CRLF line endings into LF
   $ git config --global core.autocrlf true

* MERGE TO MASTER 
  $ git checkout master
  $ git pull
  $ git checkout -b YYYYMMDD_JB
  $ git add .; git commit -am ""
  $ git rebase master
  $ git push -f
   
### HISTORY ###

* 20181205 - Initial repository.
* 20181207 - Added complete JSON processing.
* 20181218 - Added CRC16 Method.
* 20190102 - Fixes to tag parser.
* 20190104 - Added AID and CAPK processing.
* 20190107 - Added Factory Reset Option.
* 20190115 - Added Logging Capabilities.
* 20190116 - Refactored Device Interface.
           - Merged with VP5300 device interface.
* 20190117 - Enhanced TerminalData processing.
* 20190118 - Fixes to TerminalData processing.
* 20190130 - Updated config file.
* 20190206 - Enhanced log manager.
* 20190211 - Added device-side Config Groups processing.
