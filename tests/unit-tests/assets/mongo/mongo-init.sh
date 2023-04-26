#!/bin/bash

: "${FORKED:=}"
if [ -z "${FORKED}" ]; then
    echo >&2 'mongod for initdb is going to shutdown'
    /usr/bin/mongod --pidfilepath /tmp/docker-entrypoint-temp-mongod.pid --shutdown
    echo >&2 'replica set will be initialized later'
    FORKED=1 "${BASH_SOURCE[0]}" &
    unset FORKED
    mongodHackedArgs=(:) # bypass mongod --shutdown in docker-entrypoint.sh
    return
fi

# FIXME: assume mongod listens on 127.0.0.1:27017
mongo=( mongosh --host 127.0.0.1 --port 27017 --quiet )

tries=30
while true; do
    sleep 1
    if "${mongo[@]}" --eval 'quit(0)' &> /dev/null; then
        # success!
        break
    fi
    (( tries-- ))
    if [ "$tries" -le 0 ]; then
        echo >&2
        echo >&2 'error: unable to initialize replica set'
        echo >&2
        kill -STOP 1 # initdb won't be executed twice, so fail loudly
        exit 1
    fi
done

echo 'about to initialize replica set'
# FIXME: hard-coded replica set name & member host.
"${mongo[@]}" <<-EOF
    disableTelemetry();
    var admin = db.getSiblingDB('admin');
    admin.auth("$MONGO_INITDB_ROOT_USERNAME", "$MONGO_INITDB_ROOT_PASSWORD");
    rs.initiate({_id: 'rs0', members: [{_id: 0, host: "localhost"}]});
EOF
