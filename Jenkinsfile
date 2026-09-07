pipeline {
    agent any
  
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
        stage('Detect Changes & Build & Deploy') {
            steps {
                script {
                    // Detect changed files
                    def changedFiles = sh(
                        script: 'git diff --name-only HEAD~1 HEAD',
                        returnStdout: true
                    ).trim().split('\n')

                    def services = [
                        'auth-services'        : 'foodlyauth',
                        'cart-service'         : 'foodlycart',
                        'food-service'         : 'foodlyfood',
                        'notification-service' : 'foodlynotification',
                        'order-service'        : 'foodlyorder',
                        'payment-service'      : 'foodlypayment',
                        'user-service'         : 'foodlyuser',
                        'ApiGateway'           : 'foodlyapigateway'
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
                                dir(serviceFolder) {
                                    // Build image
                                    def img = docker.build(
                                        "${DOCKERHUB_USERNAME}/${imageName}:${TAG}",
                                        "."
                                    )
                                    // Push version tag
                                    img.push("${TAG}")
                                   // Push latest
                                    img.push("latest")
                                }
                                // Update container
                                sh """
                                    docker compose pull 
                                    docker compose up -d 
                                """
                            } else {
                                echo "Skip ${serviceFolder} - no changes"
                            }


                        }


                    }


                }


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
