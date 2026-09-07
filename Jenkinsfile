pipeline {
    agent any

    environment {
        DOCKERHUB_USERNAME = 'ptrungduc1011'
        DOCKER_CREDS_ID = 'DockerHub'
        TAG = "${BUILD_NUMBER}"

        VPS_HOST = '161.248.147.31'
        VPS_USER = 'root'
        VPS_DEPLOY_PATH = '/foodlydevops'
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

                                echo "========================================"
                                echo "Building ${serviceFolder}"
                                echo "Image: ${imageName}"
                                echo "Tag: ${TAG}"
                                echo "========================================"

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

                            } else {
                                echo "Skip ${serviceFolder} - no changes"
                            }
                        }
                    }
                }
            }
        }

        stage('Deploy to VPS') {
            steps {
                script {

                    echo "Deploying to VPS..."

                    withCredentials([
                        usernamePassword(
                            credentialsId: 'vps-root-password',
                            usernameVariable: 'VPS_SSH_USER',
                            passwordVariable: 'VPS_SSH_PASSWORD'
                        )
                    ]) {

                        sh '''
                            sshpass -e ssh \
                                -o StrictHostKeyChecking=no \
                                "$VPS_SSH_USER@$VPS_HOST" \
                                "cd $VPS_DEPLOY_PATH && \
                                 docker compose pull && \
                                 docker compose up -d"
                        '''
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
