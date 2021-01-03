# convert_tool
This is a ROSE client data convertion tool that will take a extracted IROSE client (112_112na) and convert the data into sql and lua files to support the osirose-new server project

# before building - DO NOT FORGET!
    CMD> git submodule update --init --recursive

The rest is just convert_tools.sln -> build

# How to use
1. Create folder inside client named convert_tool
2. Copy build files under src/bin/debug into the folder above
3. Run convert_tool.exe
4. Copy srv_data/scripts into server files
5. Load item_db.sql + skill_db.sql into your database

You're good to go!
