# Unity Infinite Reusable Scroll View
Este é um teste técnico para um desenvolvedor sênior em Unity focado na criação e otimização de um ScrollView infinito e reciclável. Ele deve ser capaz de lidar com milhares de elementos sem perda de desempenho, com a adição de uma mecânica de pull-to-refresh.

A cena "Testavel" já está configurada com todo o setup necessário para esclarecer dúvidas. 

O relatório sobre otimização pode ser encontrado no último tópico desse documento.

Você pode acompanhar todo o processo de commits do projeto, com descrições das ações feitas durante o processo clicando [aqui](https://github.com/MarceloAlves8799/Unity-Infinite-Reusable-Scroll-View/commits?author=MarceloAlves8799)

# Criação do banco de dados para nomes
1. Na aba "Project", clique com o botão direito na pasta "Scriptable Objects". A opção "Create" aparecerá, permitindo criar um Scriptable Object chamado "Database". Clique nele para criar um novo Scriptable Object que você poderá configurar. Caso contrário, há um Scriptable Object padrão disponível para uso na mesma pasta.
 
2. Adicione os nomes desejados à lista do DatabaseSO. Se preferir gerar 10.000 nomes automaticamente, basta selecionar o DatabaseSO na aba "Inspector", clicar nos três pontos alinhados e escolher "Generate Database". Veja a imagem abaixo para referência:

![image](https://github.com/MarceloAlves8799/Unity-Infinite-Reusable-Scroll-View/assets/48249122/10618557-4163-406d-81f1-fcd80404ad15)


# Setup da cena do Infinite Reusable Scroll View
1. Na sua cena, crie um Canvas para conter o Scroll View. Após ter criado ele, crie um game object vazio, ajuste o âncoramento dele da forma que fique melhor na tela e nomeio como Infinite Reusable Scroll View.

2. Crie um Scroll View como objeto filho do Infinite Reusable Scroll View, ancore e ajuste o tamanho da maneira que fique melhor e remova as scrollbars verticais e horizontais.

3. No game Object Content, dentro de Viewport que fica dentro do Scroll View, adicione dois componentes, Vertical Layout Group e mude o Child Alignment para Middle Center, e Content Size Fitter, lembre-se de definir o Vertical Fit para Preferred Size.

4. Arraste o prefab ScrollView Element, localizado na pasta Prefabs, para dentro do Content, multiplique ele até ficar com uma quantidade de elementos que cubra toda a tela e ajuste no Vertical Layout Group o Spacing para ficar com um espaçamento que se fique adequado. Após configurado pode apagar todos os prefabs dentro do Content.

5. Adicione como objeto filho do Infinite Reusable Scroll View, o prefab Object Pool, localizado na pasta Prefabs. No script ObjectPoll do elemento adicionado, trate de referênciar o prefab do ScrollView Element no campo Prefab To Instantiate.

![image](https://github.com/MarceloAlves8799/Unity-Infinite-Reusable-Scroll-View/assets/48249122/ded51fd3-e430-4fdc-baf7-22c5ab69aa48)

# Implementando o InfiniteReusableScrollView.cs
1. Adicione o script InfiniteReusableScrollView.cs no game object pai Infinite Reusable Scroll View.
2. Referencie o seu DatabaseSO criado para o campo Db Element Name.
3. Referencie o prefab de ScrollView Element para o campo Element Prefab Rect Transform.
4. Ajuste o Scroll Threshold para determinar qual a distância os novos elementos devem ser gerados com base na posição de rolagem.
5. Ajuste o Generation Time para definir o intervalo de tempo entre as instâncias que os elementos do scroll vão surgir.
6. Defina o Disable Offset para determinar a distância da borda do viewport na qual os elementos devem ser desativados.
7. Especifique o Min Amount To Start Disable para definir o número mínimo de elementos antes de considerar a devolução de elementos ao pool de objetos.

![image](https://github.com/MarceloAlves8799/Unity-Infinite-Reusable-Scroll-View/assets/48249122/38bd58b6-1609-45dd-b2b7-fecd03625f55)

# Adicionando a mecânica de Pull to Request
1. Arraste o prefab PullToRefresh Feedback, localizado na pasta Prefabs, como objeto filho do Infinite Reusable Scroll View. É importante ele se encontrar desativado na cena.
2. Adicione o script PullToRefreshHandler no game object Infinite Reusable Scroll View.
3. No campo Pull To Refresh Animator, referencie o PullToRefresh Feedback que você colocou na cena.
4. No campo Distance From Top, adicione a distância que você quer que o game Object Content fique travado durante a animação de feedback.

![image](https://github.com/MarceloAlves8799/Unity-Infinite-Reusable-Scroll-View/assets/48249122/4e36d70c-1f1a-4b22-8ac5-102c76b217fe)


# Conclusão
Ao concluir as etapas acima, você pode clicar em Play e o Scroll View vai ser populado com uma quantidade de elementos que varia da altura da sua tela, podendo ser scrollado infinitamente e quando no topo da lista é ativado a função Pull to Request.

































