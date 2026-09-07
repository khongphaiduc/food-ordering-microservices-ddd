pipeline {
    agent any

    options {
        skipDefaultCheckout(true)
    }

    environment {
        DOCKERHUB_USERNAME = 'ptrungduc1011'
        DOCKER_CREDS_ID = 'DockerHub'
        TAG = "${BUILD_NUMBER}"
    }

    stages {

        stage('Checkout Code') {
            steps {
                checkout scm
            }
        }

        stage('Detect Changes & Build & Push') {
            steps {
                script {

                    def changedFiles = sh(
                        script: 'git diff --name-only HEAD~1 HEAD',
                        returnStdout: true
                    ).trim().split('\n')

                    def services = [
                        'auth-services'         : 'foodlyauth',
                        'cart-service'          : 'foodlycart',
                        'food-service'          : 'foodlyfood',
                        'notification-service'  : 'foodlynotification',
                        'order-service'         : 'foodlyorder',
                        'payment-service'       : 'foodlypayment',
                        'user-service'          : 'foodlyuser',
                        'ApiGateway'            : 'foodlyapigateway'
                    ]

                    docker.withRegistry(
                        'https://index.docker.io/v1/',
                        DOCKER_CREDS_ID
                    ) {

                        services.each { serviceFolder, imageName ->

                            def isChanged = changedFiles.any { file ->
                                file.startsWith("${serviceFolder}/")
                            }

                            if (isChanged) {

                                echo "Building ${serviceFolder}"

                                dir(serviceFolder) {

                                    def img = docker.build(
                                        "${DOCKERHUB_USERNAME}/${imageName}:${TAG}",
                                        "."
                                    )

                                    img.push("${TAG}")
                                    img.push("latest")
                                }

                            } else {
                                echo "Skip ${serviceFolder} - no changes"
                            }
                        }
                    }
                }
            }
        }

        stage('Deploy') {
            steps {
                sh '''
                    cd /foodlydevops

                    docker compose down

                    docker compose pull

                    docker compose up -d
                '''
            }
        }
    }

    post {
        always {
            sh '''
                docker image prune -f || true
            '''
        }
    }
}
