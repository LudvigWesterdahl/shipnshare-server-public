#!/bin/bash

readonly PROG_PATH=${0}
readonly PROG_DIR=${0%/*}
readonly PROG_NAME=$(basename $0)
#readonly LOCAL_IP=$(ifconfig | grep "inet " | grep -v 127.0.0.1 | cut -d \  -f2 | head -n 1)
readonly LOCAL_IP=$(ifconfig | grep "inet " | grep -v 127.0.0.1 | grep -v "\-->" | cut -d " " -f2)

readonly WEB_PATH="ShipWithMeWeb/ShipWithMeWeb.csproj"
readonly CORE_PATH="ShipWithMeCore/ShipWithMeCore.csproj"
readonly INFRASTRUCTURE_PATH="ShipWithMeInfrastructure/ShipWithMeInfrastructure.csproj"
readonly TEST_PATH="ShipWithMeTest/ShipWithMeTest.csproj"
readonly SQL_CONTAINER="swm-sqlserver"


notify() {
    echo "${PROG_NAME}: $1"
    return 0
}

print_usage() {

    echo "Usage: ${PROG_NAME} [option]"
    echo
    echo "Examples:"
    echo " * Generate certificate without the ip address"
    echo " do --gen-cert \"\" --not-ip"
    echo
    echo "Options:"
    echo " --api,             Starts the server in HTTPS mode"
    echo " --http,            If combined with --api then starts the server in HTTP mode"
    echo " --not-localhost,   If combined with --api then server does not listen to localhost"
    echo " --not-ip,          If combined with --api then server does not listen to the ip or --gen-cert the ip is not added to cert"
    echo " --port, <num>      If combined with --api then starts the server in the given port, defaults to 5050"
    echo " --db, <cmd>        [init, show, start, stop, clean] local database"
    echo " --mig-add, <name>  Creates a database migration"
    echo " --mig-del,         Removes the last database migration"
    echo " --mig-list,        Lists all migrations"
    echo " --clean-docker,    Cleans all local docker images/containers"
    echo " --install-tools,   Installs required dotnet command line tools"
    echo " --dev-cert,        Generates the developer certificate"
    echo " --gen-cert, <addr> Generates the certificates with password abc123, defaults to shipnshare.local"
    echo " --pcert, <name>    Prints the given certificate certificate file (cert.pem)"
    echo " --secrets, <file>  Sets the user secrets from the provided json file"
    echo " --cdsecrets,       Creates the initial secrets.develop.json file"
    echo " --cdpsecrets,      Creates the initial secrets.production.json file"
    echo " --test,            Runs the tests."
    echo " -v, --verbose      Extra logging"

    return 0
}

main() {

    declare -ri NUM_ARGS=0

    if [ "${1}" == "-h" ] || [ "${1}" == "--help" ]; then
	print_usage
	return 0
    fi

    if [ $# -le ${NUM_ARGS} ]; then 
	notify "try '${PROG_NAME} -h' or '${PROG_NAME} --help' for more information"
	return 1
    fi

    declare HAS_API_FLAG="false"
    declare HAS_HTTP_FLAG="false"
    declare HAS_NOT_LOCALHOST_FLAG="false"
    declare HAS_NOT_IP_FLAG="false"
    declare HAS_PORT_FLAG="false"
    declare PORT_FLAG=""
    declare HAS_DB_FLAG="false"
    declare DB_FLAG=""
    declare HAS_MIG_ADD_FLAG="false"
    declare MIG_ADD_FLAG=""
    declare HAS_MIG_DEL_FLAG="false"
    declare HAS_MIG_LIST_FLAG="false"
    declare HAS_CLEAN_DOCKER_FLAG="false"
    declare HAS_INSTALL_TOOLS_FLAG="false"
    declare HAS_DEV_CERT_FLAG="false"
    declare HAS_GEN_CERT_FLAG="false"
    declare GEN_CERT_FLAG=""
    declare HAS_PCERT_FLAG="false"
    declare PCERT_FLAG=""
    declare HAS_SECRETS_FLAG="false"
    declare SECRETS_FLAG=""
    declare HAS_CDSECRETS_FLAG="false"
    declare HAS_CDPSECRETS_FLAG="false"
    declare HAS_TEST_FLAG="false"
    declare HAS_VERBOSE_FLAG="false"
    declare VERBOSE_FLAG=""

    declare -i I=$NUM_ARGS
    I=I+1
    while [ $I -le $# ]; do
	case ${!I} in
	    "--api")
		HAS_API_FLAG="true"
		;;
	    "--http")
		HAS_HTTP_FLAG="true"
		;;
	    "--not-localhost")
		HAS_NOT_LOCALHOST_FLAG="true"
		;;
	    "--not-ip")
		HAS_NOT_IP_FLAG="true"
		;;
	    "--port")
		I=I+1
		HAS_PORT_FLAG="true"
		PORT_FLAG=${!I}
		;;
	    "--db")
		I=I+1
		HAS_DB_FLAG="true"
		DB_FLAG=${!I}
		;;
	    "--mig-add")
		I=I+1
		HAS_MIG_ADD_FLAG="true"
		MIG_ADD_FLAG=${!I}
		;;
	    "--mig-del")
		HAS_MIG_DEL_FLAG="true"
		;;
	    "--mig-list")
		HAS_MIG_LIST_FLAG="true"
		;;
	    "--clean-docker")
		HAS_CLEAN_DOCKER_FLAG="true"
		;;
	    "--install-tools")
		HAS_INSTALL_TOOLS_FLAG="true"
		;;
	    "--dev-cert")
		HAS_DEV_CERT_FLAG="true"
		;;
	    "--gen-cert")
		I=I+1
		HAS_GEN_CERT_FLAG="true"
		GEN_CERT_FLAG=${!I}
		;;
	    "--pcert")
		I=I+1
		HAS_PCERT_FLAG="true"
		PCERT_FLAG=${!I}
		;;
	    "--secrets")
		I=I+1
		HAS_SECRETS_FLAG="true"
		SECRETS_FLAG=${!I}
		;;
	    "--cdsecrets")
		HAS_CDSECRETS_FLAG="true"
		;;
	    "--cdpsecrets")
		HAS_CDPSECRETS_FLAG="true"
		;;
	    "--test")
		HAS_TEST_FLAG="true"
		;;
	    "-v")
		HAS_VERBOSE_FLAG="true"
		VERBOSE_FLAG="--verbose"
		;;
	    "--verbose")
		HAS_VERBOSE_FLAG="true"
		VERBOSE_FLAG="--verbose"
		;;
	    *)
		notify "${!I} does not match any supported option"
		;;
	esac
	I=I+1
    done

    if [ "${HAS_API_FLAG}" == "true" ]; then
	
	declare PROT="https"
	
	if [ "${HAS_HTTP_FLAG}" == "true" ]; then
	    PROT="http"
	fi

	declare PORT="5050"
	if [ "${HAS_PORT_FLAG}" == "true" ]; then
	    PORT=${PORT_FLAG}
	fi
	
	
	declare LOCAL_URL="${PROT}://${LOCAL_IP}:${PORT}"		
	declare LOCALHOST_URL="${PROT}://127.0.0.1:${PORT}"
	declare URLS=${LOCAL_URL}";"${LOCALHOST_URL}

	if [ "${HAS_NOT_IP_FLAG}" == "true" ]; then
	    URLS=${LOCALHOST_URL}
	fi
	
	if [ "${HAS_NOT_LOCALHOST_FLAG}" == "true" ]; then
	    URLS=${LOCAL_URL}
	fi
	
	if [ "${HAS_VERBOSE_FLAG}" == "true" ]; then
	    notify "environment=develop"
	    notify "urls=${URLS}"
	    notify "token-url=${LOCALHOST_URL}"
	fi

	dotnet run --project ${WEB_PATH} --environment="develop" --urls "${URLS}"
	#dotnet run --project ${WEB_PATH} --launch-profile="develop" --urls "${LOCAL_URL};${LOCALHOST_URL}" --token-url "${LOCALHOST_URL}"
	#dotnet run --project ${WEB_PATH} --launch-profile="develop"
	return 0
    fi

    if [ "${HAS_DB_FLAG}" == "true" ]; then
	if [ "${DB_FLAG}" == "" ]; then
	    notify "missing cmd for --db"
	    return 1
	fi
	
	if [ "${DB_FLAG}" == "init" ]; then
	    notify "initializing database..."
	    export ASPNETCORE_ENVIRONMENT=develop
	    #dotnet ef database update --project ${INFRASTRUCTURE_PATH} ${VERBOSE_FLAG}
	    dotnet ef database update --startup-project ${WEB_PATH} --project ${INFRASTRUCTURE_PATH} ${VERBOSE_FLAG}
	    notify "done!"
	    return 0
	elif [ "${DB_FLAG}" == "show" ]; then
	    docker container ls | grep ${SQL_CONTAINER} || echo "stopped"
	    return 0
	elif [ "${DB_FLAG}" == "start" ]; then
	    notify "starting docker container..."
	    if [ "${HAS_VERBOSE_FLAG}" == "true" ]; then
		docker-compose up --detach
	    else
		docker-compose up --detach 1>/dev/null 2>/dev/null
	    fi
	    notify "done!"
	    return 0
	elif [ "${DB_FLAG}" == "stop" ]; then
	    notify "stopping docker continer..."
	    if [ "${HAS_VERBOSE_FLAG}" == "true" ]; then
		docker container stop ${SQL_CONTAINER}
	    else
		docker container stop ${SQL_CONTAINER} 1>/dev/null 2>/dev/null
	    fi
	    notify "done!"
	    return 0
	elif [ "${DB_FLAG}" == "clean" ]; then
	    notify "deleting local db data..."
	    docker container stop ${SQL_CONTAINER} 1>/dev/null 2>/dev/null
	    docker container rm ${SQL_CONTAINER} 1>/dev/null 2>/dev/null
	    rm -rf .${SQL_CONTAINER}-data/
	    notify "done!"
	    return 0
	else
	    notify "unrecognized --db argument '${DB_FLAG}'"
	    return 1    
	fi
    fi

    if [ "${HAS_MIG_ADD_FLAG}" == "true" ]; then
	if [ "${MIG_ADD_FLAG}" == "" ]; then
	    notify "missing name for --mig-add"
	    return 1
	fi

	notify "creating ${MIGRATE_FLAG} migration"
	export ASPNETCORE_ENVIRONMENT=develop
	dotnet ef migrations add ${MIG_ADD_FLAG} --startup-project ${WEB_PATH} --project ${INFRASTRUCTURE_PATH} ${VERBOSE_FLAG}
	notify "done!"
	return 0
    fi

    if [ "${HAS_MIG_DEL_FLAG}" == "true" ]; then
	notify "removing the last migration in ${INFRASTRUCTURE_PATH}"
	export ASPNETCORE_ENVIRONMENT=develop
	dotnet ef migrations remove --startup-project ${WEB_PATH} --project ${INFRASTRUCTURE_PATH} ${VERBOSE_FLAG}
	notify "done!"
	return 0
    fi

    if [ "${HAS_MIG_LIST_FLAG}" == "true" ]; then
	export ASPNETCORE_ENVIRONMENT=develop
	dotnet ef migrations list --startup-project ${WEB_PATH} --project ${INFRASTRUCTURE_PATH} ${VERBOSE_FLAG}
	notify "done!"
	return 0
    fi	

    if [ "${HAS_CLEAN_DOCKER_FLAG}" == "true" ]; then
	notify "cleaning up docker containers and images..."
	if [ "${HAS_VERBOSE_FLAG}" == "true" ]; then
	    notify "stop \$(docker ps -a -q)"
	    docker stop $(docker ps -a -q)
	    notify "rm \$(docker ps -a -q) -f"
	    docker rm $(docker ps -a -q) -f
	    notify "system prune -a -f"
	    docker system prune -a -f
	    notify "done!"
	    return 0
	else
	    docker stop $(docker ps -a -q) 1>/dev/null 2>/dev/null
	    docker rm $(docker ps -a -q) -f 1>/dev/null 2>/dev/null
	    docker system prune -a -f 1>/dev/null 2>/dev/null
	    notify "done!"
	    return 0
	fi
    fi

    if [ "${HAS_INSTALL_TOOLS_FLAG}" == "true" ]; then
    	dotnet new tool-manifest ${VERBOSE_FLAG}
	dotnet tool install dotnet-ef ${VERBOSE_FLAG}
	return 0
    fi

    if [ "${HAS_DEV_CERT_FLAG}" == "true" ]; then
	notify "cleaning old certificates..."
	dotnet dev-certs https --clean ${VERBOSE_FLAG}
	notify "generating certificate..."
	dotnet dev-certs https --export-path "certificate.crt" --no-password --trust --format "Pem" ${VERBOSE_FLAG}
	notify "checking and trusting..."
	dotnet dev-certs https --check --trust
	notify "done!"
	return 0
    fi
    
    if [ "${HAS_GEN_CERT_FLAG}" == "true" ]; then
	if [ "${GEN_CERT_FLAG}" == "" ]; then
	    notify "missing addr for --gen-cert, defaulting to shipnshare.local"
	    GEN_CERT_FLAG="shipnshare.local"
	fi

	declare CONF="req.conf"
	
	if [ -f ${CONF} ]; then
	    notify "deleting old openssl conf file..."
	    rm -rf ${CONF}
	fi
	
	if [ ! -f ${CONF} ]; then
	    notify "missing openssl conf file, creating it..."
	    echo "[req]" >> ${CONF}
	    echo "distinguished_name = req_distinguished_name" >> ${CONF}
	    echo "x509_extensions = v3_req" >> ${CONF}
	    echo "prompt = no" >> ${CONF}
	    echo "[req_distinguished_name]" >> ${CONF}
	    echo "C = SE" >> ${CONF}
	    echo "ST = SH" >> ${CONF}
	    echo "L = SomeCity" >> ${CONF}
	    echo "O = MyCompany" >> ${CONF}
	    echo "OU = MyDivision" >> ${CONF}
	    echo "CN = www.${GEN_CERT_FLAG}" >> ${CONF}
	    echo "[v3_req]" >> ${CONF}
	    echo "keyUsage = critical, digitalSignature, keyAgreement" >> ${CONF}
	    echo "extendedKeyUsage = serverAuth" >> ${CONF}
	    echo "subjectAltName = @alt_names" >> ${CONF}
	    echo "[alt_names]" >> ${CONF}
	    echo "DNS.1 = www.${GEN_CERT_FLAG}" >> ${CONF}
	    echo "DNS.2 = ${GEN_CERT_FLAG}" >> ${CONF}

	    if [ "${HAS_NOT_IP_FLAG}" == "false" ]; then
		echo "DNS.3 = ${LOCAL_IP}" >> ${CONF}
		echo "IP.1 = ${LOCAL_IP}" >> ${CONF}
	    fi
	fi
	
	openssl req -x509 -passout pass:abc123 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -config ${CONF}
	openssl pkcs12 -export -out cert.pfx -passout pass:abc123 -inkey key.pem -passin pass:abc123 -in cert.pem
	notify "done!"
	notify "don't forget to append a line /private/etc/hosts:"
	notify "<SERVER IP> ${GEN_CERT_FLAG}"
	notify "where <SERVER IP> might be 127.0.0.1"
    fi

    if [ "${HAS_PCERT_FLAG}" == "true" ]; then
	if [ "${PCERT_FLAG}" == "" ]; then
	    notify "missing name for --pcert"
	    return 1
	fi

	openssl x509 -in ${PCERT_FLAG} -text -noout
	notify "done!"
    fi

    if [ "${HAS_SECRETS_FLAG}" == "true" ]; then
	if [ "${SECRETS_FLAG}" == "" ]; then
	    notify "missing file for --secrets"
	    return 1
	fi

	cat ${SECRETS_FLAG} | dotnet user-secrets --project ${WEB_PATH} set

	if [ $? -eq 0 ]; then
	    notify "done!"
	else
	    notify "require initialization, call 'dotnet user-secrets init --project ${WEB_PATH}'"
	    return 1
	fi
    fi

    if [ "${HAS_CDSECRETS_FLAG}" == "true" ]; then
	declare SECRETS_PATH="ShipWithMeWeb/secrets.develop.json"
	if [ -f ${SECRETS_PATH} ]; then
	    notify "${SECRETS_PATH} already exists."
	    return 1
	else
	    echo "{" >> ${SECRETS_PATH}
	    echo "  \"Hostname\": \"[DOMAIN NAME, ex. shipnshare.local]\"," >> ${SECRETS_PATH}
	    echo "" >> ${SECRETS_PATH}
	    echo "  \"CertificatePath\": \"../cert.pfx\"," >> ${SECRETS_PATH}
	    echo "  \"CertificatePassword\": \"abc123\"," >> ${SECRETS_PATH}
	    echo "" >> ${SECRETS_PATH}
	    echo "  \"ConnectionStrings\": {" >> ${SECRETS_PATH}
	    echo "    \"Main\": \"Data Source=127.0.0.1,1433;User ID=SA;Password=Password123;\"" >> ${SECRETS_PATH}
	    echo "  }," >> ${SECRETS_PATH}
	    echo "" >> ${SECRETS_PATH}
	    echo "  \"Jwt\": {" >> ${SECRETS_PATH}
	    echo "    \"Secret\": \"TheSecretBelongingToShipWithMeForDevelopment!\"," >> ${SECRETS_PATH}
	    echo "    \"ValidIssuer\": \"http://localhost:5050\"," >> ${SECRETS_PATH}
	    echo "    \"ValidAudience\": \"http://localhost:5050\"," >> ${SECRETS_PATH}
	    echo "    \"ValidMinutes\": \"525600\"" >> ${SECRETS_PATH}
	    echo "  }," >> ${SECRETS_PATH}
	    echo "" >> ${SECRETS_PATH}
	    echo "  \"Email\": {" >> ${SECRETS_PATH}
	    echo "    \"Port\": \"[PORT, ex. 587]\"," >> ${SECRETS_PATH}
	    echo "    \"Server\": \"[SERVER, ex. smtp.gmail.com]\"," >> ${SECRETS_PATH}
	    echo "    \"SenderEmail\": \"[EMAIL ADDRESS]\"," >> ${SECRETS_PATH}
	    echo "    \"SenderPassword\": \"[PASSWORD, ex. gmail app password]\"," >> ${SECRETS_PATH}
	    echo "    \"SenderName\": \"[TITLE OF SENDER, ex. Indecode AB]\"" >> ${SECRETS_PATH}
	    echo "  }" >> ${SECRETS_PATH}
	    echo "}" >> ${SECRETS_PATH}
	fi
	notify "done!"
    fi

    if [ "${HAS_CDPSECRETS_FLAG}" == "true" ]; then
	declare SECRETS_PATH="ShipWithMeWeb/secrets.production.json"
	if [ -f ${SECRETS_PATH} ]; then
	    notify "${SECRETS_PATH} already exists."
	    return 1
	else
	    echo "{" >> ${SECRETS_PATH}
	    echo "  \"Hostname\": \"[DOMAIN NAME, ex. shipnshare.local]\"," >> ${SECRETS_PATH}
	    echo "" >> ${SECRETS_PATH}
	    echo "  \"CertificatePath\": \"../[CERT]\"," >> ${SECRETS_PATH}
	    echo "  \"CertificatePassword\": \"[CERT PASSWORD]\"," >> ${SECRETS_PATH}
	    echo "" >> ${SECRETS_PATH}
	    echo "  \"ConnectionStrings\": {" >> ${SECRETS_PATH}
	    echo "    \"Main\": \"[CONNECTION STRING]\"" >> ${SECRETS_PATH}
	    echo "  }," >> ${SECRETS_PATH}
	    echo "" >> ${SECRETS_PATH}
	    echo "  \"Jwt\": {" >> ${SECRETS_PATH}
	    echo "    \"Secret\": \"[SECRET]\"," >> ${SECRETS_PATH}
	    echo "    \"ValidIssuer\": \"[DOMAIN NAME]\"," >> ${SECRETS_PATH}
	    echo "    \"ValidAudience\": \"[DOMAIN NAME]\"," >> ${SECRETS_PATH}
	    echo "    \"ValidMinutes\": \"[MINUTES]\"" >> ${SECRETS_PATH}
	    echo "  }," >> ${SECRETS_PATH}
	    echo "" >> ${SECRETS_PATH}
	    echo "  \"Email\": {" >> ${SECRETS_PATH}
	    echo "    \"Port\": \"[PORT, ex. 587]\"," >> ${SECRETS_PATH}
	    echo "    \"Server\": \"[SERVER, ex. smtp.gmail.com]\"," >> ${SECRETS_PATH}
	    echo "    \"SenderEmail\": \"[EMAIL ADDRESS]\"," >> ${SECRETS_PATH}
	    echo "    \"SenderPassword\": \"[PASSWORD, ex. gmail app password]\"," >> ${SECRETS_PATH}
	    echo "    \"SenderName\": \"[TITLE OF SENDER, ex. Indecode AB]\"" >> ${SECRETS_PATH}
	    echo "  }," >> ${SECRETS_PATH}
	    echo "" >> ${SECRETS_PATH}
	    echo "  \"AdminUser\": {" >> ${SECRETS_PATH}
	    echo "    \"UserName\": \"[ADMIN USERNAME]\"," >> ${SECRETS_PATH}
	    echo "    \"Email\": \"[ADMIN EMAIL]\"," >> ${SECRETS_PATH}
	    echo "    \"Password\": \"[ADMIN PASSWORD]\"" >> ${SECRETS_PATH}
	    echo "  }" >> ${SECRETS_PATH}
	    echo "}" >> ${SECRETS_PATH}
	fi
	notify "done!"
    fi

    if [ "${HAS_TEST_FLAG}" == "true" ]; then
	notify "running tests..."
	dotnet test ${TEST_PATH}
	notify "done!"
	return 0
    fi

    return 0
}

main "${@}"
